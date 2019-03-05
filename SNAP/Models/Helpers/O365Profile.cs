using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SNAP.Models.Helpers
{
    public class O365Feature
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Purpose")]
        public string Purpose { get; set; }
    }

    public class O365Profile
    {
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Name")]
        public string ShortName { get; set; }

        [Display(Name = "License")]
        public string License { get; set; }

        [Display(Name = "Cost")]
        public string Cost { get; set; }

        [Display(Name = "Features")]
        public List<O365Feature> Features { get; set; }

        public O365Profile()
        {
            Features = new List<O365Feature>();
        }

        internal static List<O365Profile> GetProfileList()
        {
            try
            {
                List<O365Profile> o365LicenseProfiles = new List<O365Profile>();

                O365Profile profile1 = new O365Profile();
                profile1.Name = "Profile 1 - No Computer";
                profile1.ShortName = "Profile 1";
                profile1.License = "K1";
                profile1.Cost = "$33/YR";
                profile1.Features.Add(new O365Feature() { Name = "Email", Purpose = "Mail, Calendar, Contacts" });
                profile1.Features.Add(new O365Feature() { Name = "SharePoint Online", Purpose = "Collaboration & Document Management" });
                profile1.Features.Add(new O365Feature() { Name = "Office Online", Purpose = "Web-based Office Applications" });
                profile1.Features.Add(new O365Feature() { Name = "Yammer", Purpose = "Enterprise Social Network" });
                o365LicenseProfiles.Add(profile1);

                O365Profile profile2 = new O365Profile();
                profile2.Name = "Profile 2 - No Computer";
                profile2.ShortName = "Profile 2";
                profile2.License = "E1";
                profile2.Cost = "$80/YR";
                profile2.Features.AddRange(profile1.Features);
                profile2.Features.Add(new O365Feature() { Name = "OneDrive for Business", Purpose = "Online File Storage & Sharing" });
                profile2.Features.Add(new O365Feature() { Name = "Skype for Business", Purpose = "IM, Voice, and Video Collaboration" });
                o365LicenseProfiles.Add(profile2);

                O365Profile profile3 = new O365Profile();
                profile3.Name = "Profile 3 - With Computer";
                profile3.ShortName = "Profile 3";
                profile3.License = "E1/Windows";
                profile3.Cost = "$133/YR";
                profile3.Features.AddRange(profile2.Features);
                profile3.Features.Add(new O365Feature() { Name = "Windows", Purpose = "Operating System" });
                o365LicenseProfiles.Add(profile3);

                O365Profile profile4 = new O365Profile();
                profile4.Name = "Profile 4 - With Computer";
                profile4.ShortName = "Profile 4";
                profile4.License = "E3, EMS";
                profile4.Cost = "$249/YR";
                profile4.Features.AddRange(profile3.Features);
                profile4.Features.Add(new O365Feature() { Name = "Office O365", Purpose = "Desktop Office Applications" });
                o365LicenseProfiles.Add(profile4);

                return o365LicenseProfiles;
            }
            catch (Exception)
            {
                return new List<O365Profile>();
            }
        }

        internal static Dictionary<string, string> GetSelectList()
        {
            try
            {
                Dictionary<string, string> selectList = new Dictionary<string, string>();

                GetProfileList().ForEach(x => selectList.Add(x.Name, x.License));

                return selectList;
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }
    }
}