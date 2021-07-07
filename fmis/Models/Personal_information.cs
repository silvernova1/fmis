using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fmis.Models
{
    public class Personal_information
    {
        [StringLength(100)]
        public int userid { get; set; }
        public string picture { get; set; }
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
        public DateTime date_of_birth { get; set; }
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
        public int pagibig_no { get; set; }
        [StringLength(50)]
        public int phic_no { get; set; }
        [StringLength(50)]
        public int sss_no { get; set; }
        [StringLength(50)]
        public int tin_no { get; set; }
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














    }
}
