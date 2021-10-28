using System;
using System.ComponentModel;
using System.Reflection;

namespace customerportalapi.Entities.enums
{
    public enum RoleGroupTypes
    {
        [EnumGuidMapper("67445ac9-408f-4aba-940f-39be0f4974c3")]
        [Description("WP_StoreManagers")]
        StoreManager = 1,

        [EnumGuidMapper("1920d805-5807-490b-93c5-8891563a1ab2")]
        [Description("WP_Marketing")]
        Marketing = 2,

        [EnumGuidMapper("854a3ec3-6068-4b06-906a-7ea42b3a70e2")]
        [Description("WP_IT")]
        IT = 3,

        [EnumGuidMapper("d1b42ccc-90eb-4db7-b57f-832541dc8319")]
        [Description("WP_Admin")]
        Admin = 4,
    }
}
