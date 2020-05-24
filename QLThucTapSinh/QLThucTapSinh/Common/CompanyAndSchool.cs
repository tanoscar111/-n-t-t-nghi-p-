using QLThucTapSinh.Controllers;
using QLThucTapSinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLThucTapSinh.Common
{
    public class CompanyAndSchool
    {
        SQLThucTapSinhEntities database = new SQLThucTapSinhEntities();

        public bool FindPerson(string personID)
        {
            var model = database.Person.Find(personID);
            if (model == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool FindOrgan(string companiID)
        {
            var model = database.Organization.Find(companiID);
            if (model == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Create(Organization organi, int roleId)
        {
            try
            {
                Organization organ = new Organization();
                Share share = new Share();
                string comID;
                do
                {
                    comID = new Share().RandomText();
                } while (new CompanyAndSchool().FindOrgan(comID) == false);
                organ.ID = comID;
                organ.Name = organi.Name;
                organ.Address = organi.Address;
                organ.Phone = organi.Phone;
                organ.Fax = organi.Fax;
                organ.Image = organi.Image;
                organ.Logo = organi.Logo;
                organ.Note = organi.Note;
                organ.Email = organi.Email;
                organ.StartDay = DateTime.Now;
                organ.ExpiryDate = 1;
                organ.Status = true;
                organ.SendEmail = false;
                database.Organization.Add(organ);
                database.SaveChanges();
                var model = database.Organization.Find(comID);
                if(model == null)
                {
                    return false;
                }
                else
                {
                    string personID;
                    do
                    {
                        personID = share.RandomText();
                    } while (personID == comID && FindPerson(personID) == false);
                    if (roleId == 2)
                    {
                        if (share.insertPerson(personID,comID, null, 2))
                        {
                            database.Person.Find(personID).Email = organi.Email;
                            database.SaveChanges();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (share.insertPerson(personID, null, comID, 3))
                        {
                            database.Person.Find(personID).Email = organi.Email;
                            database.SaveChanges();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }   
        }

        public bool Update(Organization organ)
        {
            try
            {
                var model = database.Organization.Find(organ.ID);
                model.Name = organ.Name;
                model.Address = organ.Address;
                model.Fax = organ.Fax;
                model.Phone = organ.Phone;
                model.Image = organ.Image;
                model.Note = organ.Note;
                model.Email = organ.Email;
                model.Logo = organ.Logo;
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