using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Chevaleresk.Models;

namespace Chevaleresk.Controllers
{
    public class PanoramixController : Controller
    {
        private ChevalereskEntities db = new ChevalereskEntities();
        private JoueursRepository j = new JoueursRepository();

        // GET: Panoramix
        public ActionResult Index()
        {
            if (j.IsAlchemist(Convert.ToInt32(Session["playerID"])) && (bool)Session["playerConnected"])
            {
                var potion = db.Potion.Include(p => p.Items);
                return View(potion.ToList());
            }
            return PartialView("Error");
        }

        public ActionResult GetPotionInfo(int? potionId)
        {
            var filter = db.Potion
                .Where(p => p.Items.idItem == potionId).ToList();

            return PartialView("IndexPartial", filter);
        }

        //[ValidateAntiForgeryToken]
        public ActionResult ConcocterPotion(int id)
        {
            if (Session["playerID"] != null && (bool)Session["playerConnected"] && j.IsAlchemist(Convert.ToInt32(Session["playerID"])))
            {
                var playerID = Convert.ToInt32(Session["playerID"]);

                db.ConcocterPotion(playerID, id);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return PartialView("Error");
        }
    }
}
