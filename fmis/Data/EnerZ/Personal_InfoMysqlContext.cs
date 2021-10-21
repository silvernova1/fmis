using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fmis.Models;

namespace fmis.Data
{
    public class Personal_InfoMysqlContext
    {
        public string ConnectionString { get; set; }

        public Personal_InfoMysqlContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        public List<Personal_Information> allPersonalInformation()
        {
            List<Personal_Information> list = new List<Personal_Information>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                //MySqlCommand cmd = new MySqlCommand("SELECT pi.id,CONCAT(pi.fname, ' ', pi.mname, ' ', pi.lname) AS full_name,divs.description as division FROM pis.personal_information pi left join dts.division divs on divs.id = pi.division_id ORDER BY pi.id DESC LIMIT 10", conn);
                MySqlCommand cmd = new MySqlCommand(@"
                    select 
	                    pi.id,
                        pi.userid,
                        concat(pi.fname,' ',pi.lname) as full_name,
                        divs.description as division,
                        sec.description as section,
                        desig.description as designation
                    from 
	                    pis.personal_information pi 
	                    left join dts.division divs on divs.id = pi.division_id
                        left join dts.section sec on sec.id = pi.section_id
                        left join dts.designation desig on desig.id = pi.designation_id
                    order by 
                        concat(pi.fname,' ',pi.lname) asc
                ", conn);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Personal_Information()
                        {
                            full_name = reader["full_name"].ToString(),
                            userid = reader["userid"].ToString(),
                            division = reader["division"].ToString(),
                            section = reader["section"].ToString(),
                            designation = reader["designation"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public List<Personal_Information> forOrs_head(string userid)
        {
            List<Personal_Information> list = new List<Personal_Information>();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                //MySqlCommand cmd = new MySqlCommand("SELECT pi.id,CONCAT(pi.fname, ' ', pi.mname, ' ', pi.lname) AS full_name,divs.description as division FROM pis.personal_information pi left join dts.division divs on divs.id = pi.division_id ORDER BY pi.id DESC LIMIT 10", conn);
                MySqlCommand cmd = new MySqlCommand(@"
                    select 
	                    pi.id,
                        pi.userid,
                        concat(pi.fname,' ',pi.lname) as full_name,
                        divs.description as division,
                        sec.description as section,
                        desig.description as designation
                    from 
	                    pis.personal_information pi 
	                    left join dts.division divs on divs.id = pi.division_id
                        left join dts.section sec on sec.id = pi.section_id
                        left join dts.designation desig on desig.id = pi.designation_id
	                where 
		                pi.userid in " + userid +
                    @"order by 
                        concat(pi.fname,' ',pi.lname) asc", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Personal_Information()
                        {
                            full_name = reader["full_name"].ToString(),
                            userid = reader["userid"].ToString(),
                            division = reader["division"].ToString(),
                            section = reader["section"].ToString(),
                            designation = reader["designation"].ToString()
                        });
                    }
                }
            }

            return list;
        }

        public Personal_Information findPersonalInformation(string pis_userid)
        {
            Personal_Information personal_information = new Personal_Information();

            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                //MySqlCommand cmd = new MySqlCommand("SELECT pi.id,CONCAT(pi.fname, ' ', pi.mname, ' ', pi.lname) AS full_name,divs.description as division FROM pis.personal_information pi left join dts.division divs on divs.id = pi.division_id ORDER BY pi.id DESC LIMIT 10", conn);
                MySqlCommand cmd = new MySqlCommand(@"
                    select 
	                    pi.id,
                        pi.userid,
                        concat(pi.fname,' ',pi.mname,' ',pi.lname) as full_name,
                        divs.description as division,
                        sec.description as section,
                        desig.description as designation
                    from 
	                    pis.personal_information pi 
	                    left join dts.division divs on divs.id = pi.division_id
                        left join dts.section sec on sec.id = pi.section_id
                        left join dts.designation desig on desig.id = pi.designation_id
                    where
                        pi.userid = " + pis_userid, conn);

                var reader = cmd.ExecuteReader();
                reader.Read();

                personal_information.full_name = reader["full_name"].ToString();
                personal_information.userid = reader["userid"].ToString();
                personal_information.division = reader["division"].ToString();
                personal_information.section = reader["section"].ToString();
                personal_information.designation = reader["designation"].ToString();
            }

            return personal_information;
        }
    }
}

