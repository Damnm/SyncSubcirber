namespace EPAY.ETC.Core.Sync_Subcriber.Infrastructure.Utils
{
    public static class VehicleTypeConverter
    {
        public static string ConvertVehicleType(this string vehicleType)
        {
            if(string.IsNullOrEmpty(vehicleType)) {
                return "1";
            }
            if(vehicleType == "1" || vehicleType == "2" || vehicleType == "3" || vehicleType == "4") 
            {
                return vehicleType;
            }
            return "1";
        }
    }
}
