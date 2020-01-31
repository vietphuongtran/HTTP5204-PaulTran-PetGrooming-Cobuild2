using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetGrooming.Models.ViewModels
{
    public class UpdatePet
    {
        //Information needed to make update pet works
        //Info about one pets
        //Info about many species

        public Pet pet { get; set; }

        public List<Species> species { get; set; }
    }
}