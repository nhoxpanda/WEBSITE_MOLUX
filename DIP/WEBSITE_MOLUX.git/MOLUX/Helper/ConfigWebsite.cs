using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MOLUX.Models;

namespace MOLUX.Helper
{
    public class ConfigWebsite
    {
        private static BMSMoluxHongKongEntities _db = new BMSMoluxHongKongEntities();

        public static web_getConfigWebsite_Result Data()
        {
            var model = _db.web_getConfigWebsite().FirstOrDefault();
            return model;
        }

        public static List<web_getFooterByType_Result> TitleList(int id)
        {
            return _db.web_getFooterByType(id).ToList();
        }

        public static List<web_SocialNetworkList_Result> SocialList()
        {
            return _db.web_SocialNetworkList().ToList();
        }

        public static web_Slider AdvertisementCenter(int id)
        {
            return _db.web_Slider.Find(13);
        }

        public static web_Slider AdvertisementLeft(int id)
        {
            return _db.web_Slider.Find(14);
        }

        public static web_Slider AdvertisementRight(int id)
        {
            return _db.web_Slider.Find(15);
        }

        public static List<web_Footer> LoadContentFooter()
        {
            return _db.web_Footer.ToList();
        }
    }
}