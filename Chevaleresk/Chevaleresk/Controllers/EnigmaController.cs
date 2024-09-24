using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Chevaleresk.Models;

namespace Chevaleresk.Controllers
{
    public class EnigmaController : Controller
    {
        private ChevalereskEntities db = new ChevalereskEntities();

        // GET: Enigma
        public ActionResult Index()
        {
            if (Session["playerID"] != null && (bool)Session["playerConnected"])
            {
                var questions = db.Questions.Include(q => q.Categories);
                return View(questions.ToList());
            }
            else { return Redirect(@Url.Action("Login", "Accounts")); }
        }

        // GET: Enigma
        public ActionResult Jouer()
        {
            var questions = db.Questions.Include(q => q.Categories);
            var questionsList = questions.ToList();
            //var qstFiltre = questionsList.Where(q => q.type == type).ToList();
            Random random = new Random();
            var qstRdm = random.Next(questionsList.Count);
            var randomQuestion = questionsList[qstRdm];



            //var randomQuestion = db.questionRandom().ToList();
            return View(randomQuestion);
        }

        // GET: Enigma/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Questions questions = db.Questions.Find(id);
            if (questions == null)
            {
                return HttpNotFound();
            }
            return View(questions);
        }

        // GET: Enigma/Create
        public ActionResult Create()
        {
            ViewBag.idCategorie = new SelectList(db.Categories, "idCategories", "nomCategorie");
            return View();
        }

        // POST: Enigma/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idQuestion,enonce,type,idCategorie")] Questions questions)
        {
            if (ModelState.IsValid)
            {
                db.Questions.Add(questions);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.idCategorie = new SelectList(db.Categories, "idCategories", "nomCategorie", questions.idCategorie);
            return View(questions);
        }

        // GET: Enigma/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Questions questions = db.Questions.Find(id);
            if (questions == null)
            {
                return HttpNotFound();
            }
            ViewBag.idCategorie = new SelectList(db.Categories, "idCategories", "nomCategorie", questions.idCategorie);
            return View(questions);
        }

        // POST: Enigma/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idQuestion,enonce,type,idCategorie")] Questions questions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(questions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.idCategorie = new SelectList(db.Categories, "idCategories", "nomCategorie", questions.idCategorie);
            return View(questions);
        }

        // GET: Enigma/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Questions questions = db.Questions.Find(id);
            if (questions == null)
            {
                return HttpNotFound();
            }
            return View(questions);
        }

        // POST: Enigma/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Questions questions = db.Questions.Find(id);
            db.Questions.Remove(questions);
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


        public ActionResult GetEnigmaQuestion(string type)
        {
            var questions = db.Questions.Include(q => q.Categories);
            var questionsList = questions.ToList();

            Random random = new Random();
            List<Questions> qstFiltre;

            const string typeRandom = "R";
            const string typeEasy = "F";
            const string typeMedium = "M";
            const string typeHard = "D";

            switch (type)
            {
                case typeEasy:
                    qstFiltre = questionsList.Where(q => q.type == "F").ToList();
                    break;
                case typeMedium:
                    qstFiltre = questionsList.Where(q => q.type == "M").ToList();
                    break;
                case typeHard:
                    qstFiltre = questionsList.Where(q => q.type == "D").ToList();
                    break;
                case typeRandom:
                    qstFiltre = questionsList;
                    break;
                default:
                    qstFiltre = questionsList;
                    break;
            }

            //if (type == typeEasy)
            //{
            //    qstFiltre = questionsList.Where(q => q.type == "F").ToList();
            //}
            //if (type == typeMedium)
            //{
            //    qstFiltre = questionsList.Where(q => q.type == "M").ToList();
            //}
            //if (type == typeHard)
            //{
            //    qstFiltre = questionsList.Where(q => q.type == "D").ToList();
            //}
            //else
            //{
            //    qstFiltre = questionsList;
            //}


            var qstRdm = random.Next(qstFiltre.Count() );
            var randomQuestion = qstFiltre[qstRdm];

            return PartialView("QuestionPartial", randomQuestion);
        }

        public ActionResult ValidateEnigmaQuestion(int idJoueur, int idReponse, string typeQuestion)
        {
            try
            {
                //Joueurs playerBefore = db.Joueurs.Where(j => j.idJoueur == idJoueur).FirstOrDefault();
                //bool Alchemist
                
                const string typeEasy = "F";
                const string typeMedium = "M";
                const string typeHard = "D";

                Reponses rep = db.Reponses.Where(r => r.idReponse == idReponse).FirstOrDefault();
                Questions question = db.Questions.Where(q=>q.idQuestion == rep.idQuestion).FirstOrDefault();
                Categories categorie = db.Categories.Where(c=>c.idCategories == question.idCategorie).FirstOrDefault();
                var idCategorie = categorie.idCategories;

                var estCorrect = new ObjectParameter("estCorrect", typeof(int));
                db.vérifierReponse(idReponse, idJoueur, estCorrect);
                bool reponse = (bool)estCorrect.Value;
                int winningAmmount = 0;
                switch (typeQuestion)
                {
                    case typeEasy:
                        winningAmmount = 50;
                        break;
                    case typeMedium:
                        winningAmmount = 100;
                        break;
                    case typeHard:
                        winningAmmount = 200;
                        break;
                }

                TempData["playerWon"] = reponse;
                if (reponse)
                {
                    if(idCategorie == 1)
                    {
                        db.MonterAlchimiste(idJoueur);
                        //bool playerAfter = 
                    }
                    //var stats = db.Statistiques.Where(s => s.idJoueur == idJoueur).FirstOrDefault();
                    //if(stats != null)
                    //{
                    //    db.augmenterStats(idJoueur);
                    //}
                    //else
                    //{
                    //    Statistiques newStats = new Statistiques();
                    //    newStats.idJoueur = idJoueur;
                    //    newStats.nbQuestionReussi = 1;
                    //    newStats.nbQuestionEchoue = 0;
                    //    db.Statistiques.Add(newStats);
                    //    db.SaveChanges();
                    //}
                    TempData["winningMessage"] = $"Vous avez gagné {winningAmmount} écus!";
                }
                //else
                //{
                //    var stats = db.Statistiques.Where(s => s.idJoueur == idJoueur).FirstOrDefault();
                //    if (stats != null)
                //    {
                //        db.diminuerStats(idJoueur);
                //    }
                //    else
                //    {
                //        Statistiques newStats = new Statistiques();
                //        newStats.idJoueur = idJoueur;
                //        newStats.nbQuestionEchoue = 1;
                //        newStats.nbQuestionReussi = 0;
                //        db.Statistiques.Add(newStats);
                //        db.SaveChanges();
                //    }
                //}
                db.ajouterStatsEnigma(idJoueur, reponse);
                return PartialView("ValidatePartial");
            }
            catch(Exception ex)
            {
                return PartialView("Error");
            }
        }
    }
}
