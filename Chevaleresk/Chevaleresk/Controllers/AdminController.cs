using Chevaleresk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chevaleresk.Controllers
{
    public class AdminController : Controller
    {
        private ChevalereskEntities db = new ChevalereskEntities();
        private JoueursRepository jRepo = new JoueursRepository();

        // GET: Admin
        public ActionResult Index()
        {
            if (Session["playerID"] != null && (bool)Session["playerConnected"] && (bool)Session["isAdmin"])
            {
                return View();
            }
            else
            {
                return PartialView("Error");
            }
        }

        [HttpGet]
        public ActionResult Solde()
        {
            if (Session["playerID"] != null && (bool)Session["playerConnected"] && (bool)Session["isAdmin"])
            {
                var joueurs = db.Joueurs.ToList().OrderBy(j => j.alias);
                return View(joueurs);
            }
            return PartialView("Error");
        }

        public ActionResult AugmenterSolde(int idJoueur, int nouveauSolde)
        {
            if (Session["playerID"] == null || !(bool)Session["playerConnected"] || !(bool)Session["isAdmin"])
            {
                return PartialView("Error");
            }

            if (ModelState.IsValid)
            {
                if(db.Joueurs.Find(idJoueur) == null)
                {
                    TempData["NotificationType"] = "error";
                    TempData["NotificationMessage"] = $"Veuillez réessayer plus tard.";
                    TempData["NotificationTitle"] = "Erreur de traitement";
                    return RedirectToAction("Solde");
                }
                if(nouveauSolde < 0)
                {
                    TempData["NotificationType"] = "error";
                    TempData["NotificationMessage"] = $"Veuillez entrer un solde valide.";
                    TempData["NotificationTitle"] = "Solde invalide";
                    return RedirectToAction("Solde");
                }

                db.Joueurs.Find(idJoueur).solde = nouveauSolde;
                db.SaveChanges();
                TempData["NotificationType"] = "success";
                TempData["NotificationMessage"] = $"Solde de {jRepo.GetAlias(idJoueur)} modifié avec succès!";
                TempData["NotificationTitle"] = "Modification réussi!";
                return RedirectToAction("Solde");
                //db.AugmenterEcus(idJoueur, nouveauSolde);
            }
            TempData["NotificationType"] = "error";
            TempData["NotificationMessage"] = $"Veuillez entrer un solde valide.";
            TempData["NotificationTitle"] = "Solde invalide";
            return RedirectToAction("Solde");
        }
    }
}