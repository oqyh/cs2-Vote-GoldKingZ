using Newtonsoft.Json;
using Vote_GoldKingZ.Config;

namespace Vote_GoldKingZ;

public class Json_VoteKick
{
    public class PersonData
    {
        public ulong PlayerSteamID { get; set; }
        public string? PlayerName { get; set; }
        public string? PlayerIPAddress { get; set; }
        public DateTime DateAndTime { get; set; }
        public int RestrictedForXMins { get; set; }
        public string? Reason { get; set; }
    }
    public static void SaveToJsonFile(ulong PlayerSteamID, string PlayerName, string PlayerIPAddress, DateTime DateAndTime, int RestrictedForXMins, string Reason, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (!Directory.Exists(Fpath))
            {
                Directory.CreateDirectory(Fpath);
            }

            if (!File.Exists(Fpathc))
            {
                File.WriteAllText(Fpathc, "[]");
            }

            List<PersonData> allPersonsData;
            string jsonData = File.ReadAllText(Fpathc);
            allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

            PersonData existingPerson = allPersonsData.Find(p => p.PlayerSteamID == PlayerSteamID)!;

            if (existingPerson != null)
            {
                existingPerson.DateAndTime = DateAndTime;
                existingPerson.Reason = Reason;
            }
            else
            {
                PersonData newPerson = new PersonData { PlayerSteamID = PlayerSteamID, PlayerName = PlayerName, PlayerIPAddress = PlayerIPAddress, DateAndTime = DateAndTime, RestrictedForXMins = RestrictedForXMins, Reason = Reason };
                allPersonsData.Add(newPerson);
            }
            
            allPersonsData.RemoveAll(p => (DateTime.Now - p.DateAndTime).TotalMinutes > RestrictedForXMins);

            string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
            try
            {
                File.WriteAllText(Fpathc, updatedJsonData);
            }
            catch
            {
                // Handle exception
            }
        }
        catch
        {
            // Handle exception
        }
    }

    public static PersonData RetrievePersonDataById(ulong targetId, int Time, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                PersonData targetPerson = allPersonsData.Find(p => p.PlayerSteamID == targetId)!;

                if (targetPerson != null && (DateTime.Now - targetPerson.DateAndTime<= TimeSpan.FromDays(Time)))
                {
                    return targetPerson;
                }
                else if (targetPerson != null)
                {
                    allPersonsData.Remove(targetPerson);
                    string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                    try
                    {
                        File.WriteAllText(Fpathc, updatedJsonData);
                    }
                    catch
                    {
                        // Handle exception
                    }
                }
            }
        }
        catch
        {
            // Handle exception
        }
        return new PersonData();
    }
    public static PersonData RetrievePersonDataByIp(string PlayerIPAddress, int Time, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                PersonData targetPerson = allPersonsData.Find(p => p.PlayerIPAddress == PlayerIPAddress)!;

                if (targetPerson != null && (DateTime.Now - targetPerson.DateAndTime<= TimeSpan.FromDays(Time)))
                {
                    return targetPerson;
                }
                else if (targetPerson != null)
                {
                    allPersonsData.Remove(targetPerson);
                    string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                    try
                    {
                        File.WriteAllText(Fpathc, updatedJsonData);
                    }
                    catch
                    {
                        // Handle exception
                    }
                }
            }
        }
        catch
        {
            // Handle exception
        }
        return new PersonData();
    }
    public static PersonData RetrievePersonDataByReason(string reason, int Time, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                PersonData targetPerson = allPersonsData.Find(p => p.Reason == reason)!;

                if (targetPerson != null && (DateTime.Now - targetPerson.DateAndTime<= TimeSpan.FromDays(Time)))
                {
                    return targetPerson;
                }
                else if (targetPerson != null)
                {
                    allPersonsData.Remove(targetPerson);
                    string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                    try
                    {
                        File.WriteAllText(Fpathc, updatedJsonData);
                    }
                    catch
                    {
                        // Handle exception
                    }
                }
            }
        }
        catch
        {
            // Handle exception
        }
        return new PersonData();
    }
    public static void RemoveAnyByReason(string Reason, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                List<PersonData> kickedPersons = allPersonsData.Where(p => p.Reason == Reason).ToList();

                foreach (var kickedPerson in kickedPersons)
                {
                    allPersonsData.Remove(kickedPerson);
                }

                string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                try
                {
                    File.WriteAllText(Fpathc, updatedJsonData);
                }
                catch
                {
                    // Handle exception
                }
            }
        }
        catch
        {
            // Handle exception
        }
    }
    public static bool IsPlayerIPRestricted(string PlayerIPAddress, int Time, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                allPersonsData.RemoveAll(p => (DateTime.Now - p.DateAndTime).TotalMinutes > Time);

                string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                try
                {
                    File.WriteAllText(Fpathc, updatedJsonData);
                }
                catch
                {
                    // Handle exception
                }

                foreach (var personData in allPersonsData)
                {
                    if (personData.PlayerIPAddress != null && personData.PlayerIPAddress == PlayerIPAddress)
                    {
                        return true;
                    }
                }
            }
        }
        catch
        {
            // Handle exception
        }
        return false;
    }
    public static bool IsPlayerSteamIDRestricted(ulong PlayerId, int Time, string filename)
    {
        string cookiesFilePath = Configs.Shared.CookiesFolderPath!;
        string Fpath = Path.Combine(cookiesFilePath, "../../plugins/Vote-GoldKingZ/Cookies/");
        string Fpathc = Path.Combine(cookiesFilePath, Fpath + filename);
        try
        {
            if (File.Exists(Fpathc))
            {
                string jsonData = File.ReadAllText(Fpathc);
                List<PersonData> allPersonsData = JsonConvert.DeserializeObject<List<PersonData>>(jsonData) ?? new List<PersonData>();

                allPersonsData.RemoveAll(p => (DateTime.Now - p.DateAndTime).TotalMinutes > Time);

                string updatedJsonData = JsonConvert.SerializeObject(allPersonsData, Formatting.Indented);
                try
                {
                    File.WriteAllText(Fpathc, updatedJsonData);
                }
                catch
                {
                    // Handle exception
                }
                foreach (var personData in allPersonsData)
                {
                    if (personData.PlayerIPAddress != null && personData.PlayerSteamID == PlayerId)
                    {
                        return true;
                    }
                }
            }
        }
        catch
        {
            // Handle exception
        }
        return false;
    }
}