using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Chevaleresk.Models;
using Microsoft.Ajax.Utilities;

namespace Chevaleresk.Controllers
{
    public class MarketController : Controller
    {
        private ChevalereskEntities db = new ChevalereskEntities();
        private JoueursRepository jRepo = new JoueursRepository();
        private string msg = "";

        //GET: Market
        public ActionResult Index()
        {
            var magasin = db.Magasin.Include(m => m.Items);
            return View(magasin);
        }


        //POST: Filtre par type d'items

        public ActionResult GetFilteredItems(bool arme, bool armure, bool potion, bool element)
        {
            var filter = db.Magasin.Include(m => m.Items);

            var selectedFilters = new List<string>();
            if (arme)
            {
                selectedFilters.Add("A");
            }
            if (armure)
            {
                selectedFilters.Add("C");
            }
            if (potion)
            {
                selectedFilters.Add("P");
            }
            if (element)
            {
                selectedFilters.Add("E");
            }

            if (selectedFilters.Any())
            {
                filter = filter.Where(m => selectedFilters.Contains(m.Items.typeItem));
            }

            var filteredItems = filter.ToList();

            return PartialView("IndexPartial", filteredItems);
        }

        //POST: Filtre Recherche

        public ActionResult SearchItems(string searchString)
        {

            //Regex pour s'assurer que la recherche est valide (pas de caractères spéciaux)
            Regex regex = new Regex("^[a-zA-Z0-9]*$");
            if (!regex.IsMatch(searchString))
            {
                searchString = string.Empty;
            }

            var items = db.Items.Where(i => i.nom.Contains(searchString)).ToList();

            var filter = items.Select(item => item.Magasin).ToList();
            return PartialView("IndexPartial", filter);
        }

        // POST: Filtre prix

        public ActionResult GetItemsbyPrice(int? minPrice, int? maxPrice)
        {
            if (minPrice == null) { minPrice = 0; };
            if (maxPrice == null) { maxPrice = 10000; };

            var filter = db.Magasin
                .Where(m => m.Items.prix >= minPrice && m.Items.prix <= maxPrice)
                .ToList();

            return PartialView("IndexPartial", filter);
        }


        // GET: Market/Details/5
        public ActionResult Details(int? id, string status = "")
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Magasin magasin = db.Magasin.Find(id);
            if (magasin == null)
            {
                return HttpNotFound();
            }
            ViewBag.MsgError = status;
            return View(magasin);
        }

        // GET: Market/Create
        public ActionResult Create()
        {
            ViewBag.idItem = new SelectList(db.Items, "idItem", "nom");
            return View();
        }

        // POST: Market/Create
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idItem")] Magasin magasin)
        {
            if (ModelState.IsValid)
            {
                db.Magasin.Add(magasin);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idItem = new SelectList(db.Items, "idItem", "nom", magasin.idItem);
            return View(magasin);
        }

        // GET: Market/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Magasin magasin = db.Magasin.Find(id);
            if (magasin == null)
            {
                return HttpNotFound();
            }
            ViewBag.idItem = new SelectList(db.Items, "idItem", "nom", magasin.idItem);
            return View(magasin);
        }

        // POST: Market/Edit/5
        // Afin de déjouer les attaques par survalidation, activez les propriétés spécifiques auxquelles vous voulez établir une liaison. Pour 
        // plus de détails, consultez https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idItem")] Magasin magasin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(magasin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idItem = new SelectList(db.Items, "idItem", "nom", magasin.idItem);
            return View(magasin);
        }

        // GET: Market/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Magasin magasin = db.Magasin.Find(id);
            if (magasin == null)
            {
                return HttpNotFound();
            }
            return View(magasin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AjouterCritique(int idItem, int idJoueur, DateTime date, string Commentaire = "", int stars = 0)
        {
            Magasin magasin = db.Magasin.Find(idItem);
            if (magasin == null)
            {
                return PartialView("Error");
            }

            if ( stars < 1 || stars > 5)
            {
                //msg =  "Veuillez inscrire un commentaire et/ou un nombre d'étoile valide.";
                TempData["NotificationType"] = "error";
                TempData["NotificationMessage"] = $"Veuillez inscrire un nombre d'étoile valide. ";
                TempData["NotificationTitle"] = "Avis invalide";
                return RedirectToAction("Details", new { id = idItem, status = msg });
                //return PartialView("Error");
            }
            //db.Critique.Add(new Critique { idItem = idItem, idJoueur = idJoueur, nbEtoile = stars, date = date, Commentaire = Commentaire });


            db.laisserUnCommentaire(idItem, idJoueur, stars, Commentaire);
            TempData["NotificationType"] = "success";
            TempData["NotificationMessage"] = $"Votre avis a été ajouté avec succès!";
            TempData["NotificationTitle"] = "Ajout réussi!";
            return RedirectToAction("Details", new { id = idItem });
        }

        public ActionResult RetirerCommentaire(int idCommentaire)
        {
            Critique critique = db.Critique.Find(idCommentaire);
            var idJoueur = Convert.ToInt32(Session["playerID"]);

            if (critique == null)
            {
                return PartialView("Error");
            }

            var idItem = critique.idItem;

            if (critique.idJoueur == Convert.ToInt32(Session["playerID"]) || jRepo.IsAdmin(Convert.ToInt32(Session["playerID"])))
            {
                db.retirerCommentaire(idCommentaire);
                TempData["NotificationType"] = "success";
                TempData["NotificationMessage"] = $"Avis de {jRepo.GetAlias(idJoueur)} supprimé avec succès!";
                TempData["NotificationTitle"] = "Suppression réussi!";

                return RedirectToAction("Details", new { id = idItem });
            }

            return PartialView("Error");
        }



        // POST: Market/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Magasin magasin = db.Magasin.Find(id);
            db.Magasin.Remove(magasin);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
