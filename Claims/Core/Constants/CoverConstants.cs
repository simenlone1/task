namespace Claims.Domain.Constants;

public static class CoverConstants
{
   public static decimal MAX_DAMAGE_COST = 100000.0m;
   public static decimal COVER_BASE_RATE = 1250.0m;

   public static decimal[][] COVER_DISCOUNTS = new[]
   {
       new[] { 1.10m, 1.05m, 1.02m }, //Yacht
       new[] { 1.20m, 1.18m, 1.17m }, //PassengerShip
       new[] { 1.30m, 1.28m, 1.27m }, //ContainerShip
       new[] { 1.30m, 1.28m, 1.27m }, //BulkCarrier 
       new[] { 1.50m, 1.48m, 1.47m }, //Tanker 
   };

}