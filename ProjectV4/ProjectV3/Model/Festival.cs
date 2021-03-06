﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ProjectV3.Model
{
    class Festival: IDataErrorInfo
    {
        #region 'Fields en implemented methodes'
        public static DateTime start = DateTime.Now;
        public static DateTime End = DateTime.Now;
        private String _name;
        [Required(ErrorMessage="De naam voor het festival is verplicht.")]
        [StringLength(50,MinimumLength =2, ErrorMessage="De naam moet tussen de 2 en 45 karakters hebben.")]
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        
        public DateTime _startDate;
        [Required(ErrorMessage="De startdatum is verplicht.")]
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value;
            start = _startDate;
            }
        }
        private DateTime _endDate;
        [Required(ErrorMessage="De einddatum is verplicht.")]
        public DateTime EndDate
        {
            get { return _endDate; }
            set { _endDate = value;
            End = _endDate;
            }
        }
        private String _imagelink;
        
        public String ImageLink
        {
            get { return _imagelink; }
            set { _imagelink = value; }
        }
        public string Error
        {
            get { return "Object Festival is niet geldig."; }
        }

        public string this[string columnName]
        {
            get
            {
                try
                {
                    object value = this.GetType().GetProperty(columnName).GetValue(this);
                    Validator.ValidateProperty(value, new ValidationContext(this, null, null)
                    {
                        MemberName = columnName
                    });
                }
                catch (ValidationException ex)
                {
                    return ex.Message;
                }
                return String.Empty;
            }
        }
        public bool IsValid()
        {
            return Validator.TryValidateObject(this, new ValidationContext(this, null, null),
            null, true);
        }
        #endregion
        #region 'Ophalen data en verwerken'
        public static Festival GetFestival()
        {
            try
            {
                Festival festi = new Festival();
                string sql = "SELECT * from Festival";
                DbDataReader reader = Database.GetData(sql);
                reader.Read();
                if (!DBNull.Value.Equals(reader["FestivalEnd"]))
                {
                    festi.EndDate = Convert.ToDateTime(reader["FestivalEnd"].ToString());
                }
                else { festi.EndDate = DateTime.Now.Date; }
                festi.ImageLink = reader["Picture"].ToString();
                festi.Name = reader["FestivalNaam"].ToString();
                if (!DBNull.Value.Equals(reader["Start"]))
                {
                    festi.StartDate = Convert.ToDateTime(reader["Start"].ToString());
                }
                else festi.StartDate = DateTime.Now.Date;
                reader.Close();
                return festi;
            }
            catch
            {
                return new Festival();
            }
        }
        public void SaveFestival()
        {
            DbParameter name = Database.AddParameter("@Name", this.Name);
            DbParameter Start = Database.AddParameter("@Start", MakeDateForSQL(this.StartDate));
            DbParameter End = Database.AddParameter("@End", MakeDateForSQL(this.EndDate));
            DbParameter id = Database.AddParameter("@id", 1);
            DbParameter Pic = Database.AddParameter("@Pic", this.ImageLink);
            string sql = "UPDATE festival SET FestivalNaam=@Name, Start=@Start, FestivalEnd=@End, Picture=@Pic WHERE Id=@id";
            int iAffectedRows = Database.ModifyData(sql, name, Start, End, Pic, id);
        }
        private string MakeDateForSQL(DateTime dateTime)
        {
            string date = Convert.ToString(dateTime.Year) + "/";
            date += Convert.ToString(dateTime.Month) + "/";
            date += Convert.ToString(dateTime.Day);
            return date;
        }
        #endregion
    }

}
