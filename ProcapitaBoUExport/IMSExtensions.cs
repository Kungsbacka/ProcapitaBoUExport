using System;
using System.Linq;
using ProcapitaBoUExport.ProcapitaIMS;

namespace ProcapitaBoUExport
{
    public static class IMSExtensions
    {
        public static string GetFieldValue(this ArrayOfExtensionFieldExtensionField[] extension, string fieldName)
        {
            if (extension == null)
            {
                return null;
            }
            var extensionField = extension.FirstOrDefault(field => field.fieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(extensionField?.fieldValue))
            {
                return null;
            }
            return extensionField.fieldValue;
        }

        public static string GetNamePart(this NameDType name, string partType)
        {
            if (name == null)
            {
                return null;
            }
            var partName = name.partName.FirstOrDefault(part => part.namePartType.Equals(partType, StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrWhiteSpace(partName?.namePartValue))
            {
                return null;
            }
            return partName.namePartValue;
        }

        public static InstitutionRoleDTypeInstitutionRoleType GetPrimaryRole(this InstitutionRoleDType[] roles)
        {
            if (roles == null)
            {
                return InstitutionRoleDTypeInstitutionRoleType.Other;
            }
            var primaryRole = roles.FirstOrDefault(role => role.primaryRoleType);
            if (primaryRole == null)
            {
                return InstitutionRoleDTypeInstitutionRoleType.Other;
            }
            return primaryRole.institutionRoleType;
        }
    }
}
