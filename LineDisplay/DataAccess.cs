using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Sql;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows;

namespace LineDisplay
{
    public class DataAccess
    {
        String ConnectionString;
        

        #region CONSTRUCTORS
        public DataAccess()
        {
         
                ConnectionString = ConfigurationSettings.AppSettings["DBConnectionString"];
                
                try
                {
                    SqlConnection sqlConnection = new SqlConnection(ConnectionString);
                    

                    
                    sqlConnection.Open();
                    
                    sqlConnection.Close();
                    
                }
                catch (Exception e)
                {
                    throw e;
                }

        }


        #endregion



        bool checkOutput()
        {
            Random r = new Random();
            if( r.Next() % 2 == 0 )
                return false;
            else return true;
        }


        public String getLine()
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT value from Config where[key]='Name'";
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            return (String)dt.Rows[0][0];
        }

        public int getPulseCount()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT value from Config where[key]='PulseCount'";
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            return Convert.ToInt32((String)dt.Rows[0][0]);
        }

        public int getGroup()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT value from Config where[key]='Group'";
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            return Convert.ToInt32((String)dt.Rows[0][0]);
        }

        public MACHINE_STATUS getStatus()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT value from Config where[key]='Status'";
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();
            int status = Convert.ToInt32((String)dt.Rows[0][0]);

            return (MACHINE_STATUS)status;
        }

        public void getMachineDetails(int machine,out String Name, out int GroupId,
            out double rmax, out double omin, out double omax, out double gmin, out double gmax,out double tms,out double mpIpD,
            out double stopCloseDuration)
        {
            Name = String.Empty;
            GroupId = 0;
            rmax = omin  = omax = gmin = gmax =tms= 0;
            mpIpD = 30;
            stopCloseDuration = 3600;

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT * from Machines where Id = {0}";
            qry = String.Format(qry, machine);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            Name = (String)dt.Rows[0]["Name"];
            GroupId = (int)dt.Rows[0]["MachineGroupId"];
            rmax = (double)dt.Rows[0]["Rmax"];
            omin = (double)dt.Rows[0]["Omin"];
            omax = (double)dt.Rows[0]["Omax"];
            gmin = (double)dt.Rows[0]["Gmin"];
            gmax = (double)dt.Rows[0]["Gmax"];
            tms = (double)dt.Rows[0]["TOS"];
            mpIpD = (double)dt.Rows[0]["MPDuration"];
            stopCloseDuration = (double)dt.Rows[0]["StopCloseDuration"];

        }


        public int getMachinePulseCount(int machine)
        {
         

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT Pulses from Machines where Id = {0}";
            qry = String.Format(qry, machine);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            return (int)dt.Rows[0][0];

        }



        public int getMachine()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT value from Config where[key]='Group'";
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            return Convert.ToInt32((String)dt.Rows[0][0]);
        }



        public Project getProject(int machineId)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;

            qry = @"SELECT Projects.Id,Name,CycleTime from Projects
                      inner join ProjectMachines on ProjectMachines.Project_Id = Projects.Id
                       where ProjectMachines.Machine_Id = {0} and ProjectMachines.Active = 'Yes'";
             qry = String.Format(qry, machineId);
             SqlCommand cmd = new SqlCommand(qry, con);
             SqlDataReader dr = cmd.ExecuteReader();
             DataTable dt = new DataTable();
             dt.Load(dr);
             dr.Close();
             cmd.Dispose();
             if (dt.Rows.Count == 0)
                 return null;

            return new Project((int)dt.Rows[0]["ID"],(String)dt.Rows[0]["Name"],
                (double)dt.Rows[0]["CycleTime"]);
        }

        public ShiftCollection getShifts(int machine)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            ShiftCollection shifts= new ShiftCollection();

            String qry = String.Empty;
            qry = @"SELECT Shifts.ID,Shifts.Name,Shifts.[Start],Shifts.[End],
                    Sunday, Monday, Tuesday, Wednesday,Thursday, Friday, Saturday FROM shifts
                    inner join ShiftMachines on Shifts.Id = ShiftMachines.Shift_Id
                    where ShiftMachines.Machine_Id = {0} 
                    ORDER BY id";

            qry = String.Format(qry, machine);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                shifts.Add(new Shift((int)dt.Rows[i]["id"],(string)dt.Rows[i]["name"],
                   ((DateTime)dt.Rows[i]["Start"]), ((DateTime)dt.Rows[i]["End"]),
                   (Boolean)dt.Rows[i]["Sunday"], (Boolean)dt.Rows[i]["Monday"], (Boolean)dt.Rows[i]["Tuesday"],
                   (Boolean)dt.Rows[i]["Wednesday"], (Boolean)dt.Rows[i]["Thursday"], (Boolean)dt.Rows[i]["Friday"], (Boolean)dt.Rows[i]["Saturday"]));
            }

            con.Close();
            con.Dispose();
            return shifts;
        }

        public SessionCollection getSessions(int shift,int machine)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            SessionCollection sessions = new SessionCollection();

            String qry = String.Empty;
            qry = @"SELECT * FROM [Sessions] where Shift_Id={0} and Machine_Id={1}";

            qry = String.Format(qry, shift,machine);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sessions.Add(new Session(shift, (int)dt.Rows[i]["id"],
                    ((DateTime)dt.Rows[i]["Start"]).ToString(), ((DateTime)dt.Rows[i]["End"]).ToString()));
            }

            con.Close();
            con.Dispose();
            return sessions;
        }



        public SessionCollection getBreaks(int shift,int machine)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            SessionCollection breaks = new SessionCollection();

            String qry = String.Empty;
            qry = @"SELECT Breaks.Id,Start,[End] FROM Breaks where Shift_Id={0} and Machine_Id = {1}";

            qry = String.Format(qry, shift,machine);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Session s = new Session(shift, (int)dt.Rows[i]["Id"],
                    ((DateTime)dt.Rows[i]["Start"]).ToString(), ((DateTime)dt.Rows[i]["End"]).ToString());
                s.Isbreak = true;
                breaks.Add(s);
            }

            con.Close();
            con.Dispose();
            return breaks;
        }



        public List<Project> getProjectList(int machine)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            List<Project> projectList = new List<Project>();

            String qry = String.Empty;
            qry = @"SELECT Projects.ID , Name, CycleTime from Projects 
                    inner join ProjectMachines on Projects.Id = ProjectMachines.Project_Id 
                    where Machine_Id = {0}";

            qry = String.Format(qry, machine);

            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                projectList.Add(new Project((int)dt.Rows[i]["ID"],
                    (String)dt.Rows[i]["Name"], (double)dt.Rows[i]["CycleTime"]));
            }

            con.Close();
            con.Dispose();

            projectList.Add(new Project());
            return projectList;
        }

        public String validateOffCode(String code)
        {
            int cd = Int16.Parse(code);
            if (cd < 500)
                return "Valid Code";
            else return String.Empty;
        }


        public int getMachineInputs(DateTime from, DateTime to, int MachineID)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT Count(*) from MachineInputs where [timestamp] >= '{0}' and
                    [timestamp]<'{1}'  and Machine_Id = {2}";

            qry = String.Format(qry, from.ToString("yyyy-MM-dd HH:mm:ss"), to.ToString("yyyy-MM-dd HH:mm:ss"), MachineID);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            return (int)dt.Rows[0][0];
        }


        public void updatePlan(int plan,int machine)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"insert into [Plan]([Plan],timestamp,Machine_Id) values({0},'{1}',{2})";
            qry = String.Format(qry, plan, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), machine);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void updateActual(int actual,int machine)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"insert into actual(actual,timestamp,machine_id) values({0},'{1}',{2})";
            qry = String.Format(qry, actual, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), machine);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public Stop getStop(int machine)
        {
            Stop s;
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"select Top(1) * from [Stops] where status = 'Open' and Machine_Id={0} order by [Start] asc";
            qry = String.Format(qry, machine);


            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();
            con.Close();
            if (dt.Rows.Count == 0)
            {
                s = null;
            }
            else
            {

                s = new Stop();
                s.ID = (int)dt.Rows[0]["SlNo"];
                s.Status = "Open";
                s.From = (DateTime?)dt.Rows[0]["Start"];
                s.To = (dt.Rows[0]["End"] == DBNull.Value) ? null : (DateTime?)dt.Rows[0]["End"];
            }


            
            return s;
        }

        public Stop insertStop(DateTime now,int machine,double stopCloseDuration)
        {


            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"insert into [Stops]([Start],status,Machine_Id) values('{0}','{1}',{2})";
            qry = String.Format(qry, now.ToString("yyyy-MM-dd HH:mm:ss"), "Speed Loss", machine);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            qry = @"select Top(1) * from [Stops] order by SlNo desc";
            cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            con.Close();
            con.Dispose();

            Stop s = new Stop(stopCloseDuration);
            s.ID = (int)dt.Rows[0]["SlNo"];
            s.From = (DateTime)dt.Rows[0]["Start"];
            s.Status = (String)dt.Rows[0]["Status"];
            s.timeoutTimer.Start();
            return s;

           
        }

        public void updateStop_To()
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"update Stops set [End] = GETDATE() where SlNo = (Select Top(1)SlNo from Stops where [End] is null and [Status]<>'Speed Loss'
                        order by [Start] asc)";
            qry = String.Format(qry, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }


        public void updateStop_ToSpeedLoss()
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"update Stops set [End] = GETDATE() where SlNo = (Select Top(1)SlNo from Stops where [End] is null and [Status]='Speed Loss'
                        order by [Start] asc)";
            qry = String.Format(qry, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }


        public void CloseStop(int id)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"update Stops set Code=0,[Status]='Closed' where SlNo = {0} and [Status]='Open'";
            qry = String.Format(qry, id);
            qry = String.Format(qry, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }



        public void updateStop(Stop stop)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"update Stops set  status = '{1}',code = {2} where SlNo = {3}";
            qry = String.Format(qry, stop.To.ToString(), stop.Status,stop.Code,stop.ID);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }


        public void updateStop_Open(Stop stop)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"update Stops set  status = '{0}',code = {1} where SlNo = {2}";
            qry = String.Format(qry,  stop.Status, stop.Code, stop.ID);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }

        public void updateStop_SpeedLoss(Stop stop)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"update Stops set [End]='{0}' where SlNo = {1}";
            qry = String.Format(qry, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), stop.ID);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }

        public void updateOpenStops(int machineId)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"update Stops set [End]='{0}' ,  status = '{1}',code = {2} where status='Open' and Machine_Id={3} ";
            qry = String.Format(qry, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "Closed", 0,machineId);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }

        public void deleteOpenStops(int machineId)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"Delete from Stops  where status='Open' and Machine_Id={0}";
            qry = String.Format(qry, machineId);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }
        public String getProblem(int code, int type,int machine)
        {
            String problem = String.Empty;
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT [Description] from CommonProblems where Code= {0} and Type={1}
                    union 
                    SELECT [Description] from SpecificProblems where Code= {0}and Type={1} and Machine_Id={2}";

            qry = String.Format(qry, code,type,machine);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();
            con.Close();

            if (dt.Rows.Count == 0)
                return problem;
            else return (String)dt.Rows[0][0];
            
        }


        public int insertOff(int machine,int code)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"Begin Tran
                    insert into [OFFs]([Start],status,Machine_Id,Code) values('{0}','OPEN',{1},{2})
                    select Top(1) SlNo from [OFFs] order by SlNo desc
                    commit";
            qry = String.Format(qry, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), machine,code);
            SqlCommand cmd = new SqlCommand(qry, con);

            //cmd.ExecuteNonQuery();
            //cmd.Dispose();

            //qry = @"";
            cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            con.Close();
            con.Dispose();

            if (dt.Rows.Count > 0)
                return (int)dt.Rows[0]["SlNo"];
            else return -1;

        }


        public void updateOff(int slno)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"update OFFs set [End]='{0}' ,[status] = 'CLOSED' where SlNo={1}";
            qry = String.Format(qry, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), slno);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }

        public String getStopProblem(int code, int type,int machine)
        {
            String problem = String.Empty;
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();

            String qry = String.Empty;
            qry = @"SELECT [Description] from CommonProblems where Code= {0} and Type<>{1}
                    union 
                    SELECT [Description] from SpecificProblems where Code= {0}and Type<>{1} and Machine_Id={2}";

            qry = String.Format(qry, code,type,machine);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();
            con.Close();

            if (dt.Rows.Count == 0)
                return problem;
            else return (String)dt.Rows[0][0];
        }

        public void updateMachineInput( int machine, DateTime timestamp)
        {

            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"insert into machineInputs(machine_id,timestamp) values({0},'{1}')";
            qry = String.Format(qry, machine, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void updateProjectSession(int machine, int cur, int shift, int sessionActual,
          int sessionPlan)
        {
            if (cur == -1) return;
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"Begin tran
                    update ProjectTracker set [To] = '{3}', SessionActual = {4}, SessionPlan = {5}
                    where Machine_Id={0} and Shift_Id = {2} and Project_Id = {1}
                        and [To] is null
                    insert into ProjectTracker(Machine_Id,Shift_Id,Project_Id,[From])
                    values({0},{2},{1},'{3}')
                    commit";
            qry = String.Format(qry, machine, cur, shift,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), sessionActual, sessionPlan);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

            //qry = String.Empty;
            //qry = @"update ProjectMachines set Active='Yes' where Machine_Id = {0} and Project_id = {1}";
            //qry = String.Format(qry, machine, cur);
            //cmd = new SqlCommand(qry, con);
            //cmd.ExecuteNonQuery();
            //cmd.Dispose();

            //qry = String.Empty;
            //qry = @"')";
            //qry = String.Format(qry, machine,session, cur,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //cmd = new SqlCommand(qry, con);
            //cmd.ExecuteNonQuery();
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }


        public void closeProjectSession(int machine, int cur, int shift, int sessionActual,
         int sessionPlan)
        {
            if (cur == -1) return;
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"Begin tran
                    update ProjectTracker set [To] = '{3}', SessionActual = {4}, SessionPlan = {5}
                    where Machine_Id={0} and Shift_Id = {2} and Project_Id = {1}
                        and [To] is null
                   
                    commit";
            qry = String.Format(qry, machine, cur, shift,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), sessionActual, sessionPlan);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

           
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public void updateProject(int machine, int prev, int cur,int shift,int sessionActual,
            int sessionPlan )
        {
            if (prev == -1) return;
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"Begin tran
                    update ProjectMachines set Active='No' where Machine_Id = {0} and Project_id = {1}
                    update ProjectMachines set Active='Yes' where Machine_Id = {0} and Project_id = {2}
                    update ProjectTracker set [To] = '{4}', SessionActual = {5}, SessionPlan = {6}
                    where Machine_Id={0} and Shift_Id = {3} and Project_Id = {1} and [To] is null
                    insert into ProjectTracker(Machine_Id,Shift_Id,Project_Id,[From])
                    values({0},{3},{2},'{4}')
                    commit";
            qry = String.Format(qry, machine, prev, cur, shift, 
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),sessionActual,sessionPlan);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();

           
            cmd.Dispose();

            con.Close();
            con.Dispose();
        }

        public int HasMPInputOpen(int MachineID, int p,String start,String end)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
           

            qry = @"select SlNo, Status from [ManPower] where Machine_Id = {0} and Shift_Id={1} 
                and timestamp >= '{2}' and timestamp < '{3}' ";
            qry = String.Format(qry, MachineID, p, start, end);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            con.Close();
            con.Dispose();

            if (dt.Rows.Count > 0)
            {
                if ((String)dt.Rows[0]["Status"] == "Open")
                    return (int)dt.Rows[0]["SlNo"];
                else return -1;
            }
            else return 0;
        }

        public int InsertMPInput(int MachineID, int p)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"insert into ManPower(machine_id,shift_id,status,timestamp) values({0},{1},'{2}','{3}')";
            qry = String.Format(qry, MachineID,p,"Open", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();

            


            qry = @"select Max(SlNo) as SlNo from [ManPower] where Machine_Id = {0} and Shift_Id={1} 
                and status='Open' ";
            qry = String.Format(qry, MachineID, p);
            cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            

            con.Close();
            con.Dispose();

            return (int)dt.Rows[0]["SlNo"];
        }

        internal void updateManpower(int slno, int mp)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"update ManPower set [ManPower]= {0}, Status='{1}' where SlNo= {2} ";
            qry = String.Format(qry, mp,"Closed", slno);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
        }

        public void CloseManpowerInput(int machineId)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;
            qry = @"update Manpower set [ManPower]= 0 ,  status = 'Undefined' where status='Open' and Machine_Id={0}";
            qry = String.Format(qry, machineId);
            SqlCommand cmd = new SqlCommand(qry, con);

            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
        }

        internal int getMaxSessionPlan(int MachineID,String start,String end)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            con.Open();
            String qry = String.Empty;


            qry = @"select Max([Plan]) from [Plan] where Machine_Id = {0} 
                    and timestamp >= '{1}' and timestamp < '{2}' ";
            qry = String.Format(qry, MachineID, start, end);
            SqlCommand cmd = new SqlCommand(qry, con);
            SqlDataReader dr = cmd.ExecuteReader();
            DataTable dt = new DataTable();
            dt.Load(dr);
            dr.Close();
            cmd.Dispose();

            con.Close();
            con.Dispose();

            if (dt.Rows.Count > 0)
            {
                return (dt.Rows[0][0] == DBNull.Value) ? 0 : (int)dt.Rows[0][0] ;
                 
            }
            else return 0;
        }
    }
}
