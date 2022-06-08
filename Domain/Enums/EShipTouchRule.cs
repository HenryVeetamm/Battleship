using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum EShipTouchRule
    {
        [Display(Name = "No touch")] NoTouch,
        [Display(Name = "Corner touch")]CornerTouch,
        Yes,
    }

    public static class ShipTouchRule
    {
        public static string ToString(EShipTouchRule rule)
        {
            switch (rule)
            {
                case EShipTouchRule.NoTouch:
                    return "No touch";
                case EShipTouchRule.Yes:
                    return "Yes";
                case EShipTouchRule.CornerTouch:
                    return "Corner touch";
                default:
                    return "";
            }
        }
    }
}