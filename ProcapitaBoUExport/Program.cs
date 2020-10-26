using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Linq;
using ProcapitaBoUExport.ProcapitaIMS;
using CommandLine;

namespace ProcapitaBoUExport
{
    class Program
    {
        static Options options;

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed(o => options = o);
            if (options == null)
            {
                return;
            }
            DateTime searchDate;
            if (options.SearchDate.HasValue)
            {
                searchDate = options.SearchDate.Value.Date;
            }
            else
            {
                /* If no search date is specified and the export happens before the wokday
                 * is over (before 5:00 PM), yesterdays data is exported instead. If you schedule
                 * an export at night, after midnight, this is the behaviour you would expect.
                 */
                if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 17)
                {
                    searchDate = DateTime.Today.AddDays(-1);
                }
                else
                {
                    searchDate = DateTime.Today;
                }
            }
            Print("Search date: {0}", searchDate.ToString("yyyy-MM-dd"));
            DateTime startExecutionTime = DateTime.Now;
            // Print("Executing pre export stored procedure (if any)...");
            // ExecuteStoredProcedure("PreExport");
            List<string> allUnits;
            if (!string.IsNullOrEmpty(options.SingleUnit))
            {
                allUnits = new List<string>() { options.SingleUnit };
            }
            else if (options.Units.Any())
            {
                allUnits = options.Units.ToList();
            }
            else
            {
                allUnits = GetAllUnitNames(searchDate);
            }
            foreach (string unitName in allUnits)
            {
                StoreUnit(unitName, searchDate);
            }

            // Print("Executing post export stored procedure (if any)...");
            // ExecuteStoredProcedure("PostExport");

            Print("Time elapsed: {0}", DateTime.Now - startExecutionTime);
#if DEBUG
            Console.ReadLine();
#endif
        }

        static List<string> GetAllUnitNames(DateTime searchDate)
        {
            var unitList = new List<string>();
            using (var membershipClient = new MembershipManagementServiceSyncClient())
            {
                var membershipRequest = new readMembershipsForGroupRequest()
                {
                    groupSourcedId = new groupSourcedId()
                    {
                        identifier = "SearchDate=" + searchDate.ToString("yyyyMMdd")
                    }
                };
                var requestHeader = new syncRequestHeaderInfo1()
                {
                    messageIdentifier = Environment.MachineName + "_" + DateTime.Now.Ticks
                };
                readMembershipsForGroupResponse membershipResponse;
                membershipClient.readMembershipsForGroup(requestHeader, membershipRequest, out membershipResponse);
                foreach (var membership in membershipResponse.membershipSet)
                {
                    using (var groupClient = new GroupManagementServiceSyncClient())
                    {
                        var groupRequest = new readGroupRequest()
                        {
                            sourcedId = new sourcedId()
                            {
                                identifier = membership.member.memberSourcedId.identifier
                            }
                        };
                        requestHeader = new syncRequestHeaderInfo1()
                        {
                            messageIdentifier = Environment.MachineName + "_" + DateTime.Now.Ticks
                        };
                        readGroupResponse groupResponse;
                        groupClient.readGroup(requestHeader, groupRequest, out groupResponse);
                        unitList.Add(groupResponse.group.description.descShort);
                    }
                }
            }
            return unitList;
        }

        static void StoreUnit(string unitName, DateTime searchDate)
        {
            Print(unitName);
            var dataTable = DataTableFactory.CreateDataTable();
            using (var extensionClient = new ExtensionServiceSyncClient())
            {
                var batchRequest = new extReadBatchHeaderRequest()
                {
                    CreateSecondarySchoolUnitMembership = true,
                    RemoveUsersWithEmptyEmailGen = false,
                    syncRequestHeaderInfo = new syncRequestHeaderInfo()
                    {
                        messageIdentifier = Environment.MachineName + "_" + DateTime.Now.Ticks
                    },
                    Unit = unitName,
                    UnitDomain = "",
                    SearchDate = searchDate
                };
                var batchResponse = extensionClient.extReadBatch(batchRequest);
                Print("  Memberships: {0}", batchResponse.Enterprice.Memberships.Length);
                Print("  Persons: {0}", batchResponse.Enterprice.Persons.Length);
                Print("  Groups: {0}", batchResponse.Enterprice.Groups.Length);
                var membershipDict = new Dictionary<string, MembershipDType>(batchResponse.Enterprice.Memberships.Length);
                foreach (var membership in batchResponse.Enterprice.Memberships)
                {
                    if (membership.member.idType == MemberDTypeIdType.Group)
                    {
                        if (!membershipDict.ContainsKey(membership.member.memberSourcedId.identifier))
                        {
                            membershipDict.Add(membership.member.memberSourcedId.identifier, membership);
                        }
                    }
                }
                var groupDict = new Dictionary<string, GroupDType>(batchResponse.Enterprice.Groups.Length);
                foreach (var group in batchResponse.Enterprice.Groups)
                {
                    string id = group.extension.GetFieldValue("identifier");
                    if (id != null && !groupDict.ContainsKey(id))
                    {
                        groupDict.Add(id, group);
                    }
                }
                var personDict = new Dictionary<string, PersonDType>(batchResponse.Enterprice.Persons.Length);
                foreach (var person in batchResponse.Enterprice.Persons)
                {
                    if (!string.IsNullOrEmpty(person.sourcedId.identifier))
                    {
                        personDict.Add(person.sourcedId.identifier, person.person);
                    }
                }
                foreach (var membership in batchResponse.Enterprice.Memberships)
                {
                    if (membership.member.idType == MemberDTypeIdType.Person)
                    {
                        if (!personDict.ContainsKey(membership.member.memberSourcedId.identifier))
                        {
                            continue;
                        }
                        var row = dataTable.NewRow();
                        row["uttagsdatum"] = searchDate;
                        var person = personDict[membership.member.memberSourcedId.identifier];
                        row["personnr"] = membership.member.memberSourcedId.identifier;
                        row["fornamn"] = person?.name.GetNamePart("FirstName");
                        row["efternamn"] = person?.name.GetNamePart("LastName");
                        row["fullnamn"] = person?.name.GetNamePart("FullName");
                        row["roll"] = person?.institutionRole.GetPrimaryRole().ToString();
                        row["gatuadress"] = person?.address?.street[0];
                        row["postnummer"] = person?.address.postcode;
                        row["postort"] = person?.address.locality;
                        if (person?.tel?.Length > 0)
                        {
                            var phone = string.Join(",", person.tel.Select(t => t.telValue.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToArray());
                            if (!string.IsNullOrEmpty(phone))
                            {
                                row["telefon"] = phone;
                            }
                        }
                        row["vh1"] = person?.extension.GetFieldValue("CareholderOne");
                        row["vh2"] = person?.extension.GetFieldValue("CareholderTwo");
                        string groupIdentifier = membership.groupSourcedId.identifier;
                        while (groupDict.TryGetValue(groupIdentifier, out GroupDType group))
                        {
                            switch ((group.extension.GetFieldValue("GroupType") ?? "").ToLower())
                            {
                                case "unit":
                                    row["rektorsomrade"] = group.extension.GetFieldValue("SchoolManagerArea");
                                    row["rektorsomradeid"] = group.extension.GetFieldValue("SchoolManagerAreaCode");
                                    row["enhet"] = group.extension.GetFieldValue("UnitName");
                                    row["enhetid"] = group.extension.GetFieldValue("UnitId");
                                    break;
                                case "unitdomain":
                                    row["skolform"] = group.extension.GetFieldValue("UnitDomainName");
                                    row["skolformid"] = group.extension.GetFieldValue("UnitDomainId");
                                    row["skolformkort"] = group.extension.GetFieldValue("UnitDomainShortName");
                                    break;
                                case "department":
                                    row["avdelning"] = group.extension.GetFieldValue("DepartmentName");
                                    row["avdelningid"] = group.extension.GetFieldValue("DepartmentId");
                                    break;
                                case "studentclass":
                                    row["klass"] = group.extension.GetFieldValue("ClassName");
                                    row["klassid"] = group.extension.GetFieldValue("ClassId");
                                    row["klassperiod"] = group.extension.GetFieldValue("Period");
                                    row["arskurs"] = group.extension.GetFieldValue("Grade");
                                    break;
                                case "studentgroup":
                                    row["grupp"] = group.extension.GetFieldValue("GroupName");
                                    row["gruppid"] = group.extension.GetFieldValue("GroupId");
                                    row["grupperiod"] = group.extension.GetFieldValue("Period");
                                    break;
                                case "subject":
                                    row["amne"] = group.extension.GetFieldValue("SubjectCode");
                                    var match = Regex.Match(membership.groupSourcedId.identifier, "Subject=(?<subject>[\\d]+)");
                                    if (match.Groups["subject"] != null)
                                    {
                                        row["amneid"] = match.Groups["subject"].Value;
                                    }
                                    break;
                            }
                            if (membershipDict.ContainsKey(groupIdentifier))
                            {
                                groupIdentifier = membershipDict[groupIdentifier].groupSourcedId.identifier;
                            }
                        }
                        dataTable.Rows.Add(row);
                    }
                }
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MetaDirectory"].ConnectionString))
                {
                    conn.Open();
                    using (var bulkCopy = new SqlBulkCopy(conn))
                    {
                        bulkCopy.DestinationTableName = ConfigurationManager.AppSettings["DestinationTable"];
                        bulkCopy.WriteToServer(dataTable);
                    }
                }
            }
        }

        static void ExecuteStoredProcedure(string name)
        {
            string storedProcedure = ConfigurationManager.AppSettings[name];
            if (!string.IsNullOrEmpty(storedProcedure))
            {
                using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["MetaDirectory"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = storedProcedure;
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        static void Print(string msg, params object[] args)
        {
            if (options.Verbose)
            {
                Console.WriteLine(msg, args);
            }
        }
    }
}
