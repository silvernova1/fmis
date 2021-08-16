using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Personal_Information
    {
        [Key]
        public int id { get; set; }
        [StringLength(100)]
        public string userid { get; set; }
        public string picture { get; set; }
        [StringLength(255)]
        public string signature { get; set; }
        [StringLength(100)]
        public string fname { get; set; }
        [StringLength(100)]
        public string lname { get; set; }
        [StringLength(100)]
        public string mname { get; set; }
        [StringLength(100)]
        public string name_ext { get; set; }
        [DataType(DataType.Date)]
        /*public DateTime date_of_birth { get; set; }*/
        public Nullable<DateTime> date_of_birth { get; set; }
        [StringLength(100)]
        public string place_of_birth { get; set; }
        [StringLength(20)]
        public string sex { get; set; }
        [StringLength(20)]
        public string civil_status { get; set; }
        [StringLength(20)]
        public string citizenship { get; set; }
        [StringLength(100)]
        public string indicate_country { get; set; }
        [StringLength(20)]
        public string height { get; set; }
        [StringLength(20)]
        public string weight { get; set; }
        [StringLength(20)]
        public string blood_type { get; set; }
        [StringLength(50)]
        public string gsis_idno { get; set; }
        [StringLength(50)]
        public string gsis_polnno { get; set; }
        [StringLength(50)]
        public string pagibig_no { get; set; }
        [StringLength(50)]
        public string phic_no { get; set; }
        [StringLength(50)]
        public string sss_no { get; set; }
        [StringLength(50)]
        public string tin_no { get; set; }
        [StringLength(100)]
        public string residential_address { get; set; }
        [StringLength(100)]
        public string residential_municipality { get; set; }
        [StringLength(255)]
        public string residential_province { get; set; }
        [StringLength(100)]
        public string RHouse_no { get; set; }
        [StringLength(100)]
        public string RStreet { get; set; }
        [StringLength(255)]
        public string RSubdivision { get; set; }
        [StringLength(255)]
        public string RBarangay { get; set; }
        [StringLength(255)]
        public string RMunicipality { get; set; }
        [StringLength(255)]
        public string RProvince { get; set; }
        [StringLength(255)]
        public string Phouse_no { get; set; }
        [StringLength(255)]
        public string PStreet { get; set; }
        [StringLength(255)]
        public string PSubdivision { get; set; }
        [StringLength(255)]
        public string PBarangay { get; set; }
        [StringLength(255)]
        public string PMunicipality { get; set; }
        [StringLength(255)]
        public string PProvince { get; set; }
        [StringLength(50)]
        public string RZip_code { get; set; }
        [StringLength(50)]
        public string PZip_code { get; set; }
        [StringLength(50)]
        public string region_zip { get; set; }
        [StringLength(100)]
        public string telno { get; set; }
        [StringLength(100)]
        public string emall_address{ get; set; }
        [StringLength(100)]
        public string cellno { get; set; }
        [StringLength(100)]
        public string employee_status{ get; set; }
        [StringLength(100)]
        public string job_status { get; set; }
        [StringLength(100)]
        public string inactive_area { get; set; }
        [StringLength(100)]
        public string case_name { get; set; }
        [StringLength(100)]
        public string case_address { get; set; }
        [StringLength(100)]
        public string case_contact { get; set; }
        public Nullable<int> designation_id { get; set; }
        public Nullable<int> division_id { get; set; }
        public Nullable<int> section_id { get; set; }
        [StringLength(100)]
        public string disbursement_type { get; set; }
        [StringLength(100)]
        public string salary_charge { get; set; }
        [StringLength(100)]
        public string bbalance_cto { get; set; }
        [StringLength(100)]
        public string vacation_balance { get; set; }
        [StringLength(100)]
        public string sick_balance { get; set; }
        [StringLength(100)]
        public string sched  { get; set; }
        [StringLength(100)]
        public string account_number { get; set; }
        [StringLength(50)]
        public string region { get; set; }
        [StringLength(20)]
        public string field_status { get; set; }
        [StringLength(100)]
        public string Rsitio { get; set; }
        [DataType(DataType.Date)]
        public DateTime resigned_effectivity { get; set; }
        [StringLength(100)]
        public string Psitio { get; set; }
        [DataType(DataType.Date)]
        public DateTime created_at { get; set; }
        [DataType(DataType.Date)]
        public DateTime updated_at { get; set; }

        internal void SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
