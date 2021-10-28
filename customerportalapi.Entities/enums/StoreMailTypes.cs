using System.ComponentModel;

namespace customerportalapi.Entities.enums
{
    public enum StoreMailTypes
    {
        [Description("New signage")]
        NewSignage = 802260000,

        [Description("Old signage")]
        OldSignage = 802260001,

        [Description("Without Signage")]
        WithoutSignageOrNull = 802260002
    }
}
