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
            ArrayOfExtensionFieldExtensionField extensionField = extension.FirstOrDefault(field => field.fieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase));
            if (extensionField != null)
            {
                return string.IsNullOrWhiteSpace(extensionField.fieldValue) ? null : extensionField.fieldValue;
            }
            else
            {
                return null;
            }
        }

        public static string GetNamePart(this NameDType name, string partType)
        {
            if (name == null)
            {
                return null;
            }
            NameDTypePartName partName = name.partName.FirstOrDefault(part => part.namePartType.Equals(partType, StringComparison.OrdinalIgnoreCase));
            if (partName != null)
            {
                return string.IsNullOrWhiteSpace(partName.namePartValue) ? null : partName.namePartValue;
            }
            else
            {
                return null;
            }
        }

        public static InstitutionRoleDTypeInstitutionRoleType GetPrimaryRole(this InstitutionRoleDType[] roles)
        {
            if (roles == null)
            {
                return InstitutionRoleDTypeInstitutionRoleType.Other;
            }
            InstitutionRoleDType primaryRole = roles.FirstOrDefault(role => role.primaryRoleType);
            if (primaryRole != null)
            {
                return primaryRole.institutionRoleType;

            }
            else
            {
                return InstitutionRoleDTypeInstitutionRoleType.Other;
            }
        }
    }
}
