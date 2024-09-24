using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations.Sql;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Chevaleresk.Models;

namespace Chevaleresk.Controllers
{
    public class CartController : Controller
    {
        private string msg = "";

        private ChevalereskEntities db = new ChevalereskEntities();
        public ActionResult Index(string status = "")
        {
            if (Session["playerID"] != null && (bool)Session["playerConnected"])
            {
                int playerID = Convert.ToInt32(Session["playerID"]);

                var panier = db.Panier
                                .Include(p => p.Items)
                                .Include(p => p.Joueurs)
                                .Where(p => p.idJoueur == playerID);
                ViewBag.MsgError = status;
                return View(panier.ToList());
            }
            else
            {
                return PartialView("Error");
                //return RedirectToAction("Login");
            }
        }

        // : Cart/AddItem/5
        public ActionResult AddItem(int id)
        {
            if (Session["playerID"] != null && (bool)Session["playerConnected"])
            {
                db.incrementerQuantitePanier(Convert.ToInt32(Session["playerID"]), id, 1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else { return PartialView("Error"); }
        }
        public ActionResult RemoveItem(int id)
        {
            if (Session["playerID"] != null && (bool)Session["playerConnected"])
            {
                db.decrementQuantitePanier(Convert.ToInt32(Session["playerID"]), id, 1);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else { return PartialView("Error"); }
        }
        public ActionResult DeleteItem(int id)
        {
            if (Session["playerID"] != null && (bool)Session["playerConnected"])
            {
                db.RetirerDuPanier(Convert.ToInt32(Session["playerID"]), id);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else { return PartialView("Error"); }
        }

        //[ValidateAntiForgeryToken]
        public ActionResult AjouterItemPanier(int? id)
        {

            if (Session["playerID"] != null && (bool)Session["playerConnected"])
            {

                db.ajouterAuPanier(Convert.ToInt32(Session["playerID"]), id, 1);
                db.SaveChanges();

                TempData["NotificationType"] = "success";
                TempData["NotificationMessage"] = "L'item a été ajouté avec succès à votre panier.";
                TempData["NotificationTitle"] = "Ajout au panier réussi";

                return RedirectToAction("Index");


            }
            return PartialView("Error");
        }

        //[ValidateAntiForgeryToken]
        public ActionResult AcheterItems(int? id)
        {
            if (Session["playerID"] != null && (bool)Session["playerConnected"])
            {
                var playerID = Convert.ToInt32(Session["playerID"]);
                var itemsInInventory = db.Panier.Where(i => i.idJoueur == playerID).Select(i => new
                {
                    ItemPrice = i.Items.prix, // Prix de l'article
                    Quantity = i.qtItemPanier // Quantité de l'article dans l'inventaire
                })
        .ToList();

                // Calculer le prix total en multipliant le prix de chaque article par sa quantité
                decimal totalPrice = itemsInInventory.Sum(item => item.ItemPrice * item.Quantity);
                var player = (Joueurs)db.Joueurs.FirstOrDefault(p => p.idJoueur == playerID);
                if (player.solde < totalPrice)
                {
                    msg = "Vous n'avez pas les fonds nécéssaires pour acheter ce panier";
                    return RedirectToAction("Index", new { status = msg });
                }
                db.AcheterItemsDansPanier(Convert.ToInt32(Session["playerID"]));
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return PartialView("Error");
        }


        // GET: Cart/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Panier panier = db.Panier.Find(id);
            if (panier == null)
            {
                return HttpNotFound();
            }
            return View(panier);
        }

        // GET: Cart/Create
        public ActionResult Create()
        {
            ViewBag.idItem = new SelectList(db.Items, "idItem", "nom");
            ViewBag.idJoueur = new SelectList(db.Joueurs, "idJoueur", "alias");
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idPanier,idJoueur,idItem,qtItemPanier")] Panier panier)
        {
            if (ModelState.IsValid)
            {
                db.Panier.Add(panier);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idItem = new SelectList(db.Items, "idItem", "nom", panier.idItem);
            ViewBag.idJoueur = new SelectList(db.Joueurs, "idJoueur", "alias", panier.idJoueur);
            return View(panier);
        }

        // GET: Cart/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Panier panier = db.Panier.Find(id);
            if (panier == null)
            {
                return HttpNotFound();
            }
            ViewBag.idItem = new SelectList(db.Items, "idItem", "nom", panier.idItem);
            ViewBag.idJoueur = new SelectList(db.Joueurs, "idJoueur", "alias", panier.idJoueur);
            return View(panier);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idJoueur,idItem,qtItemPanier")] Panier panier)
        {
            if (ModelState.IsValid)
            {

                db.Entry(panier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.idItem = new SelectList(db.Items, "idItem", "nom", panier.idItem);
            //ViewBag.idJoueur = new SelectList(db.Joueurs, "idJoueur", "alias", panier.idJoueur);
            return View(panier);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddItem([Bind(Include = "idJoueur,idItem,qtItemPanier")] Panier panier, int? idItem)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Joueurs j = db.Joueurs.Find(Convert.ToInt32(Session["playerID"]));
        //        if (panier.idJoueur == j.idJoueur)
        //        {
        //            if (panier.Items.idItem == idItem)
        //            {
        //                panier.Items.qtStock++;
        //            }
        //        }
        //        db.Entry(panier).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    //ViewBag.idItem = new SelectList(db.Items, "idItem", "nom", panier.idItem);
        //    //ViewBag.idJoueur = new SelectList(db.Joueurs, "idJoueur", "alias", panier.idJoueur);
        //    return View(panier);
        //}

        // GET: Cart/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Panier panier = db.Panier.Find(id);
            if (panier == null)
            {
                return HttpNotFound();
            }
            return View(panier);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Panier panier = db.Panier.Find(id);
            db.Panier.Remove(panier);
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



