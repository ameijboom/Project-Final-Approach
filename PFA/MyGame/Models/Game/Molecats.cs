using PFA.MyGame.CatomTypes;

namespace PFA.MyGame.Models.Game
{
    public static class Molecats
    {
        private static Dictionary<string, int> Water = new();
        private static Dictionary<string, int> CalciumOxide = new();
        private static Dictionary<string, int> BakingSoda = new();
        private static Dictionary<string, int> CarbonOxide = new();
        private static Dictionary<string, int> CarbonMonoxide = new();
        private static Dictionary<string, int> Phosphate = new();
        private static Dictionary<string, int> PhosphoricAcid = new();
        private static Dictionary<string, int> DiNitrogenTrioxide = new();
        private static Dictionary<string, int> NitrogenDioxide = new();
        private static Dictionary<string, int> SodiumNitrate = new();

        public static List<Dictionary<string, int>> molecats;
        static Molecats()
        {
            Water.Add("H", 2);
            Water.Add("O", 1);

            CalciumOxide.Add("Ca", 1);
            CalciumOxide.Add("O", 1);

            BakingSoda.Add("Na", 1);
            BakingSoda.Add("H", 1);
            BakingSoda.Add("C", 1);
            BakingSoda.Add("O", 3);

			CarbonOxide.Add("C", 1);
			CarbonOxide.Add("O", 2);

			CarbonMonoxide.Add("C", 1);
			CarbonMonoxide.Add("O", 1);

			Phosphate.Add("P", 1);
			Phosphate.Add("O", 4);

			PhosphoricAcid.Add("H", 3);
			PhosphoricAcid.Add("P", 1);
			PhosphoricAcid.Add("O", 4);

			DiNitrogenTrioxide.Add("N", 2);
			DiNitrogenTrioxide.Add("O", 3);

			NitrogenDioxide.Add("N", 1);
			NitrogenDioxide.Add("O", 2);

			SodiumNitrate.Add("Na", 1);
			SodiumNitrate.Add("N", 1);
			SodiumNitrate.Add("O", 3);

            molecats = new List<Dictionary<string, int>>() {
                Water,
                CalciumOxide,
                BakingSoda,
                CarbonMonoxide,
                Phosphate,
                PhosphoricAcid,
                DiNitrogenTrioxide,
                NitrogenDioxide,
                SodiumNitrate,
            };
        }

        public static int GetCatomCount(string catom)
        {
            int count = 0;

            foreach(var molecat in molecats)
            {
                if (molecat.ContainsKey(catom))
                {
                    count += molecat[catom];
                }
            }

            return count;
        }

        public static bool ContainsCatom(Dictionary<string, int> molecat, string catom)
        {
            return molecat.ContainsKey(catom);
        }
    }
}
