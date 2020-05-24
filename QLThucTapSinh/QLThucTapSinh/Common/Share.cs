using QLThucTapSinh.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;


namespace QLThucTapSinh.Common
{
    

    public class Share
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();
        public List<Organization> listOrgan(int roleid)
        {
            List<Person> listPerson = database.Person.Where(x => x.RoleID == roleid).ToList();
            List<Organization> listOrgan = new List<Organization>();
            if(roleid == 2)
            {
                foreach (var item in listPerson)
                {
                    var model = database.Organization.Find(item.CompanyID);
                    listOrgan.Add(model);
                }
            }
            else
            {
                foreach (var item in listPerson)
                {
                    var model = database.Organization.Find(item.SchoolID);
                    listOrgan.Add(model);
                }
            }
            return listOrgan;
        }

        public bool ChangeStatus(string id, int role)
        {
            var com = database.Organization.Find(id);
                if (com.Status == true)
                {
                    ChangesStatusOrgan(id,role, true);
                    com.Status = false;
                }
                else
                {
                    ChangesStatusOrgan(id,role, false);
                    com.Status = true;
                }
                database.SaveChanges();
                return com.Status ;

        }

        public void ChangesStatusOrgan(string id, int role, bool status)
        {
            var findP = new Person();
            if (role == 2)
            {
                findP = database.Person.SingleOrDefault(x => x.CompanyID == id && x.RoleID == role);
            }
            else
            {
                findP = database.Person.SingleOrDefault(x => x.SchoolID == id && x.RoleID == role);
            }
            var findU = database.Users.SingleOrDefault(x => x.PersonID == findP.PersonID);
            if (findU != null)
            {
                findU.Status = !status;
            }
            if(role == 2)
            {
                var model = database.Person.Where(x => x.CompanyID == id && x.RoleID == 4).ToList();
                foreach (var item in model)
                {
                    if (ChangesStatusInternship(item.PersonID, status))
                    {
                        var findUser = database.Users.SingleOrDefault(x => x.PersonID == item.PersonID);
                        findUser.Status = !status;
                    }
                }
            }
            database.SaveChanges();
        }

        public bool ChangesStatusInternship(string id, bool status)
        {
            try
            {
                var model = database.InternShip.Where(x => x.PersonID == id).ToList();
                foreach (var item in model)
                {
                    item.Status = !status;
                }
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string RandomText()
        {
            Random rd = new Random();
            string TextRd = null;
            string Text;
            for (int i = 0; i < 8; i++)
            {
                Text = Convert.ToString((char)rd.Next(65, 90));
                TextRd = TextRd + Text;
            }
            return TextRd;
        }

        public bool insertPerson(string code, string company, string school, int role)
        {
            Person person = new Person();
            try
            {
                person.PersonID = code;
                person.RoleID = role;
                person.CompanyID = company;
                person.SchoolID = school;
                database.Person.Add(person);
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdatePerson(Person person)
        {
            try
            {
                var model = database.Person.Find(person.PersonID);
                model.LastName = person.LastName;
                model.FirstName = person.FirstName;
                model.Address = person.Address;
                model.Birthday = person.Birthday;
                model.Gender = person.Gender;
                model.Phone = person.Phone;
                model.Image = person.Image;
                model.Email = person.Email;
                database.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}