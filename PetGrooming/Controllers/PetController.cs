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
using PetGrooming.Models.ViewModels;
using System.Diagnostics;

namespace PetGrooming.Controllers
{
    public class PetController : Controller
    {
        /*
        These reading resources will help you understand and navigate the MVC environment
 
        Q: What is an MVC controller?

        - https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/controllers-and-routing/aspnet-mvc-controllers-overview-cs

        Q: What does it mean to "Pass Data" from the Controller to the View?

        - http://www.webdevelopmenthelp.net/2014/06/using-model-pass-data-asp-net-mvc.html

        Q: What is an SQL injection attack?

        - https://www.w3schools.com/sql/sql_injection.asp

        Q: How can we prevent SQL injection attacks?

        - https://www.completecsharptutorial.com/ado-net/insert-records-using-simple-and-parameterized-query-c-sql.php

        Q: How can I run an SQL query against a database inside a controller file?

        - https://www.entityframeworktutorial.net/EntityFramework4.3/raw-sql-query-in-entity-framework.aspx
 
         */
        private PetGroomingContext db = new PetGroomingContext();

        // GET: Pet
        public ActionResult List()
        {
            //How could we modify this to include a search bar?
            List<Pet> pets = db.Pets.SqlQuery("Select * from Pets").ToList();
            return View(pets);
           
        }

        // GET: Pet/Details/5
        public ActionResult Show(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Pet pet = db.Pets.Find(id); //EF 6 technique
            Pet pet = db.Pets.SqlQuery("select * from pets where petid=@PetID", new SqlParameter("@PetID",id)).FirstOrDefault();
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }
        //Search bar added not sucessfully
        [HttpPost]
        public ActionResult Search(string searchkey)
        {
            Debug.WriteLine("I am finding with name " + searchkey);
            //run the query
            string query = "select * from pets where PetName = %@searchkey% or Weight = %@searchkey% or color = %@searchkey% or Notes = %@searchkey";
            //parameterized query
            SqlParameter[] sqlparams = new SqlParameter[1];
            sqlparams[0] = new SqlParameter("@searchkey", searchkey);
            //execute
            db.Pets.SqlQuery(query, sqlparams);
            return RedirectToAction("List");
        }
        //Possible bugs: searckey not pulled in or/and command to select statement is wrong
        //Possible solution 1 to search bar: Search is basically show but with different query
        //Result: Failed because search return a list
        /*public ActionResult Search(string searchkey)
        {
            if (searchkey == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
          
            Pet pet = db.Pets.SqlQuery("select * from pets where PetName = %@searchkey% or Weight = %@searchkey% or color = %@searchkey% or Notes = %@searchkey");
            if (pet == null)
            {
                return HttpNotFound();
            }
            return View(pet);
        }*/
        //THE [HttpPost] Means that this method will only be activated on a POST form submit to the following URL
        //URL: /Pet/Add
        [HttpPost]
        public ActionResult Add(string PetName, Double PetWeight, String PetColor, int SpeciesID, string PetNotes)
        {
            //1: Gather data for species
            //The variable name  MUST match the name in the form 

            Debug.WriteLine("I am adding a pet with name " + PetName) ;

            //2: Write the query
            string query = "insert into pets (PetName, Weight, color, SpeciesID, Notes) values (@PetName,@PetWeight,@PetColor,@SpeciesID,@PetNotes)";
            
            //Parameterized query
            SqlParameter[] sqlparams = new SqlParameter[5]; //0,1,2,3,4 pieces of information to add
            //each piece of information is a key and value pair
            sqlparams[0] = new SqlParameter("@PetName",PetName);
            sqlparams[1] = new SqlParameter("@PetWeight", PetWeight);
            sqlparams[2] = new SqlParameter("@PetColor", PetColor);
            sqlparams[3] = new SqlParameter("@SpeciesID", SpeciesID);
            sqlparams[4] = new SqlParameter("@PetNotes",PetNotes);

            //3: Run the query
            db.Database.ExecuteSqlCommand(query, sqlparams);

            
            //4: return to the List
            return RedirectToAction("List");
        }


        public ActionResult Add()
        {
            //Return all species to a mark up
            List<Species> species = db.Species.SqlQuery("select * from Species").ToList();

            return View(species);
        }

        public ActionResult Update(int id)
        {
            //need information about a particular pet
            Pet selectedpet = db.Pets.SqlQuery("select * from pets where petid = @id", new SqlParameter("@id",id)).FirstOrDefault();
            //Need infor from species
            List<Species> species = db.Species.SqlQuery("select * from species").ToList();
            UpdatePet viewmodel = new UpdatePet();
            viewmodel.pet = selectedpet;
            viewmodel.species = species;

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Update(int id, string PetName, string PetColor, double PetWeight, string PetNotes)
        {

            Debug.WriteLine("I am trying to edit a pet's name to "+PetName+" and change the weight to "+PetWeight.ToString());

            //logic for updating the pet in the database goes here
            //write the query
            string query = "update pets set PetName=@PetName, color= @PetColor, Weight=@PetWeight, Notes=@PetNotes where petid=@id"; //new SqlParameter("@id", id)).FirstOrDefault();
            //Parameterized query
            SqlParameter[] sqlparams = new SqlParameter[5];            
            sqlparams[0] = new SqlParameter("@PetName", PetName);
            sqlparams[1] = new SqlParameter("@PetWeight", PetWeight);
            sqlparams[2] = new SqlParameter("@PetColor", PetColor);
            sqlparams[3] = new SqlParameter("@PetNotes", PetNotes);
            sqlparams[4] = new SqlParameter("@id", id);

            db.Database.ExecuteSqlCommand(query, sqlparams);
            //return to List view
            return RedirectToAction("List");

        }


        //return the view selected pet when deleting when delete through Post method
        /*public ActionResult Show_Delete(int id)
        {     
            Pet selectedpet = db.Pets.SqlQuery("select * from pets where petid = @id", new SqlParameter("@id", id)).FirstOrDefault();
            return View(selectedpet);
        }*/
        //[HttpPost]
        public ActionResult Delete(int id)
        {
            Debug.WriteLine("Deleting a pet with id =" + id.ToString());
            //write the query
            string query = "delete from pets where petid=@id";
            //Parameterized query
            SqlParameter[] sqlparams = new SqlParameter[1];
            sqlparams[0] = new SqlParameter("@id", id);
            db.Database.ExecuteSqlCommand(query, sqlparams);
            //return to List view
            return RedirectToAction("List");            
        }
        //(optional) Delete


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
