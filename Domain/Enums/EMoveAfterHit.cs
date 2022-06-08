using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum EMoveAfterHit
    {
        [Description("Same player")]SamePlayer,
        [Display(Name = "Other player")]OtherPlayer,
    }
    
    public static class MoveAfterHit
    {
        public static string ToString(EMoveAfterHit rule)
        {
            switch (rule)
            {
                case EMoveAfterHit.SamePlayer:
                    return "Same player";
                case EMoveAfterHit.OtherPlayer:
                    return "Other player";
                default:
                    return "";
            }
        }
    }
}