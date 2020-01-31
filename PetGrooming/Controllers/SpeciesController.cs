using System;
using System.Collections.Generic;
using System.Data;
//required for SqlParameter class
using System.Data.SqlClient;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PetGrooming.Data;
using PetGrooming.Models;
using System.Diagnostics;

namespace PetGrooming.Controllers
{
    public class SpeciesController : Controller
    {
        private PetGroomingContext db = new PetGroomingContext();
        // GET: Species
        public ActionResult Index()
        {
            return View();
        }

        //TODO: Each line should be a separate method in this class
        // List
        public ActionResult List()
        {
            //what data do we need?
            List<Species> myspecies = db.Species.SqlQuery("Select * from species").ToList();

            return View(myspecies);
        }

        // Show
        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Species species = db.Species.SqlQuery("select * from species where speciesid=@SpeciesID", new SqlParameter("@SpeciesID", id)).FirstOrDefault();
            if (species == null)
            {
                return HttpNotFound();
            }
            return View(species);
        }
        [HttpPost]
        public ActionResult Add(string SpeciesName)
        {
            //query
            string query = "insert into species (Name) values (@SpeciesName)";
            //Parameterized query
            SqlParameter[] sqlparams = new SqlParameter[1];
            sqlparams[0] = new SqlParameter("@SpeciesName", SpeciesName);
            //Execute the query
            db.Database.ExecuteSqlCommand(query, sqlparams);
            //return to List
            return RedirectToAction("List");
        }
        public ActionResult Add()
        { 

            List<Species> species = db.Species.SqlQuery("select * from Species").ToList();
            return View(species);
        }

        // Update
        // [HttpPost] Update
        public ActionResult Update(int id)
        {
            //need information about a particular pet
            Species selectedspecies = db.Species.SqlQuery("select * from species where speciesid=@SpeciesID", new SqlParameter("@SpeciesID", id)).FirstOrDefault();

            return View(selectedspecies);
        }

        [HttpPost]
        public ActionResult Update(int id, string SpeciesName)
        {                       
            //write the query
            string query = "update species set Name = @SpeciesName where SpeciesID = @id";
            //Parameterized query
            SqlParameter[] sqlparams = new SqlParameter[2];
            sqlparams[0] = new SqlParameter("@SpeciesName", SpeciesName);
            sqlparams[1] = new SqlParameter("@id", id);

            db.Database.ExecuteSqlCommand(query, sqlparams);
            //return to List view
            return RedirectToAction("List");

        }
        public ActionResult Delete(int id)
        {
            Debug.WriteLine("Deleting species with id =" + id.ToString());
            //write the query
            //before deleting the species deleting all Pets belong to the species
            string query1 = "delete from pets where speciesid=@id";
            string query2 = "delete from species where speciesid=@id";
            //Parameterized query
            SqlParameter[] sqlparams = new SqlParameter[1];
            sqlparams[0] = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(query1, sqlparams);
            db.Database.ExecuteSqlCommand(query2, sqlparams);
            //return to List view
            return RedirectToAction("List");
        }
        // (optional) delete
        // [HttpPost] Delete
    }
}