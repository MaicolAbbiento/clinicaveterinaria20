﻿using clinicaveterinaria20.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace clinicaveterinaria20.Controllers
{
    [Authorize(Roles = "Farmacista")]
    public class magazzinoController : Controller
    {
        private Model1 database = new Model1();

        public List<SelectListItem> ListaArmadietti
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                List<Armadietti> lista = new List<Armadietti>();
                lista = database.Armadietti.ToList();
                foreach (Armadietti p in lista)
                {
                    SelectListItem item = new SelectListItem { Text = $"{p.codice}", Value = $"{p.idarmadietto}" };
                    list.Add(item);
                }
                return list;
            }
        }

        public List<SelectListItem> ListaBrand
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                List<Brand> lista = new List<Brand>();
                lista = database.Brand.ToList();
                foreach (Brand p in lista)
                {
                    SelectListItem item = new SelectListItem { Text = $"{p.nome}", Value = $"{p.idbrand}" };
                    list.Add(item);
                }
                return list;
            }
        }

        public List<SelectListItem> ListaUtilizzi
        {
            get
            {
                List<SelectListItem> list = new List<SelectListItem>();
                List<Utilizzi> lista = new List<Utilizzi>();
                lista = database.Utilizzi.ToList();
                foreach (Utilizzi p in lista)
                {
                    SelectListItem item = new SelectListItem { Text = $"{p.descrizioni}", Value = $"{p.idutilizzo}" };
                    list.Add(item);
                }
                return list;
            }
        }

        // GET: magazzino

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult magazzino()
        {
            return View();
        }

        [HttpPost]
        public JsonResult magazion(string nome)
        {
            List<Prodotti> prodotto = new List<Prodotti>();
            if (nome == "")
            {
                prodotto = database.Prodotti.ToList();
            }
            else
            {
                prodotto = database.Prodotti.Where((a) => a.nome == nome).ToList();
            }

            List<ModelloProdotto> list = new List<ModelloProdotto>();
            if (prodotto.Count > 0)
            {
                foreach (var item in prodotto)
                {
                    ModelloProdotto modello = new ModelloProdotto();
                    modello.nome = item.nome;
                    modello.costo = item.costo;
                    modello.idprodotto = item.idprodotto;
                    modello.tipologia = item.tipologia;
                    modello.foto = item.foto;
                    modello.quantita = item.quantita;
                    modello.costo = item.costo;
                    modello.casetto = item.Cassetto.ncassetto;
                    modello.armadietto = item.Cassetto.Armadietti.codice;
                    modello.invendita = item.invendita;

                    list.Add(modello);
                }
            }
            else
            {
                ModelloProdotto modello = new ModelloProdotto();
                modello.nome = "prodotto insesistente";
                list.Add(modello);
            }
            return Json(list);
        }

        [HttpGet]
        public ActionResult aggiugiAziende()
        {
            return View();
        }

        [HttpPost]
        public ActionResult aggiugiAziende(Brand b)
        {
            if (ModelState.IsValid)
            {
                Brand brand = database.Brand.FirstOrDefault((a) => a.piva == b.piva && a.nome == b.nome);
                if (brand == null)
                {
                    database.Brand.Add(b);
                    database.SaveChanges();
                    TempData["successo"] = "Azienda aggiunta all'elenco";
                    return RedirectToAction("aggiugiAziende");
                }
                else
                {
                    ViewBag.errore = "Azienda gia' presente";
                    return View();
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult inserisciprodottoinmagazino()
        {
            ViewBag.Armadietti = ListaArmadietti;
            ViewBag.Brand = ListaBrand;
            ViewBag.Utlizzi = ListaUtilizzi;
            return View();
        }

        [HttpPost]
        public ActionResult inserisciprodottoinmagazino(Prodotti p, HttpPostedFileBase foto)
        {
            if (ModelState.IsValid)
            {
                if (foto != null && foto.ContentLength > 0)
                {
                    string nomeFile = foto.FileName;
                    string pathToSave = Path.Combine(Server.MapPath("~/Content/img/uploads"), nomeFile);
                    foto.SaveAs(pathToSave);
                    p.foto = foto.FileName;
                }
                else
                {
                    p.foto = "placeholder.jpg";
                }
                Prodotti prodotto = database.Prodotti.FirstOrDefault((a) => a.nome == p.nome);
                if (prodotto != null)
                {
                    ViewBag.Armadietti = ListaArmadietti;
                    ViewBag.Brand = ListaBrand;
                    ViewBag.Utlizzi = ListaUtilizzi;
                    ViewBag.Errore = "Prodotto già presente";
                    return View();
                }
                Prodotti prod = database.Prodotti.FirstOrDefault(a => a.Cassetto.ncassetto == p.Cassetto.ncassetto && a.Cassetto.idarmadietto == p.Cassetto.idarmadietto);
                if (prod != null)
                {
                    ViewBag.Armadietti = ListaArmadietti;
                    ViewBag.Brand = ListaBrand;
                    ViewBag.Utlizzi = ListaUtilizzi;
                    ViewBag.Errore = "Cassetto già occupato";
                    return View();
                }
                Cassetto c = database.Cassetto.FirstOrDefault(a => a.ncassetto == p.Cassetto.ncassetto && a.idarmadietto == p.Cassetto.idarmadietto);
                p.idcassetto = c.idcassetto;
                p.Cassetto = null;
                p.invendita = true;
                database.Prodotti.Add(p);
                database.SaveChanges();
                TempData["Successo"] = "Prodotto aggiunto all'elenco";
                return RedirectToAction("inserisciprodottoinmagazino");
            }
            ViewBag.Armadietti = ListaArmadietti;
            ViewBag.Brand = ListaBrand;
            ViewBag.Utlizzi = ListaUtilizzi;
            ViewBag.Errore = "Errore durante la procedura";
            return View();
        }

        [HttpGet]
        public ActionResult aggiungiArmadio()
        {
            return View();
        }

        [HttpPost]
        public ActionResult aggiungiArmadio(Armadietti a)
        {
            if (ModelState.IsValid)
            {
                Cassetto cassetto = new Cassetto();
                Armadietti arm = database.Armadietti.FirstOrDefault(e => a.codice == e.codice);
                if (arm == null)
                {
                    int n = 10;

                    database.Armadietti.Add(a);
                    database.SaveChanges();
                    Armadietti ar = database.Armadietti.ToList().Last();

                    for (int i = 0; i < n; i++)
                    {
                        cassetto.idarmadietto = ar.idarmadietto;
                        cassetto.ncassetto = i + 1;
                        database.Cassetto.Add(cassetto);
                        database.SaveChanges();
                    }
                    TempData["successo"] = "Armadio aggiunto all'elenco";
                    return RedirectToAction("aggiungiArmadio");
                }
                else
                {
                    ViewBag.errore = "Armadietto già presente";
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult EliminaProdotto(int id)
        {
            Prodotti prodotti = database.Prodotti.Find(id);
            prodotti.invendita = !prodotti.invendita;
            database.Entry(prodotti).State = EntityState.Modified;
            database.SaveChanges();
            return RedirectToAction("magazzino");
        }

        [HttpGet]
        public ActionResult ModificaProdotto(int id)
        {
            ViewBag.Armadietti = ListaArmadietti;
            ViewBag.Brand = ListaBrand;
            ViewBag.Utlizzi = ListaUtilizzi;
            Prodotti p = database.Prodotti.Find(id);
            return View(p);
        }

        [HttpPost]
        public ActionResult ModificaProdotto([Bind(Exclude = "invendita")] Prodotti p, HttpPostedFileBase foto)
        {
            Model1 db = new Model1();
            if (ModelState.IsValid)
            {
                var prodotto = database.Prodotti.Find(p.idprodotto);
                if (foto != null && foto.ContentLength > 0)
                {
                    string nomeFile = foto.FileName;
                    string pathToSave = Path.Combine(Server.MapPath("~/Content/img/uploads"), nomeFile);
                    foto.SaveAs(pathToSave);
                    p.foto = foto.FileName;
                }
                else
                {
                    p.foto = prodotto.foto;
                }

                if (p.Cassetto.ncassetto == prodotto.Cassetto.ncassetto)
                {
                    p.idcassetto = prodotto.idcassetto;
                }
                else
                {
                    Prodotti prod = database.Prodotti.FirstOrDefault(a => a.Cassetto.ncassetto == p.Cassetto.ncassetto && a.Cassetto.idarmadietto == p.Cassetto.idarmadietto);
                    if (prod != null)
                    {
                        ViewBag.Armadietti = ListaArmadietti;
                        ViewBag.Brand = ListaBrand;
                        ViewBag.Utlizzi = ListaUtilizzi;
                        ViewBag.Errore = "Cassetto già occupato";
                        return View();
                    }
                    Cassetto c = database.Cassetto.FirstOrDefault(a => a.ncassetto == p.Cassetto.ncassetto && a.idarmadietto == p.Cassetto.idarmadietto);
                    p.idcassetto = c.idcassetto;
                }
                p.Cassetto = null;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Successo"] = "Prodotto modificato";
                return RedirectToAction("ModificaProdotto");
            }
            ViewBag.Armadietti = ListaArmadietti;
            ViewBag.Brand = ListaBrand;
            ViewBag.Utlizzi = ListaUtilizzi;
            ViewBag.Errore = "Errore durante la procedura";
            return View();
        }

        public ActionResult AddUtilizzo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddUtilizzo(Utilizzi u)
        {
            if (ModelState.IsValid)
            {
                database.Utilizzi.Add(u);
                database.SaveChanges();
                TempData["successo"] = "Nuova voce aggiunta all'elenco";
                return RedirectToAction("AddUtilizzo");
            }
            ViewBag.Errore = "Errore durante la procedura";
            return View();
        }
    }
}