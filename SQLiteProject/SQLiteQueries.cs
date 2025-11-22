using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLib;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace mySQLite
{
    public class SQLiteQueries
    {
        public DbFacadeSQLite _sqlt;

        public SQLiteQueries(string dbName)
        {
            _sqlt = new DbFacadeSQLite(dbName);
        }


        #region Создание таблиц в БД
        /// <summary>
        /// Создание таблиц
        /// </summary>
        private void SaveLog(string txt, string fileTo = "testSQL.txt")
        {
            StreamWriter streamWriter = new StreamWriter(@"..\..\..\" + fileTo, false, Encoding.GetEncoding("utf-8"));
            streamWriter.WriteLine(txt);
            streamWriter.Close();
        }

        public void ClearDB()
        {
            DataTable dt = _sqlt.Execute(@"SELECT 'drop table ' || name || ';'    
                                     FROM sqlite_master
                                     WHERE type = 'table';");
            _sqlt.BeginTransaction();
            foreach (DataRow row in dt.Rows)
            {
                _sqlt.ExecuteNonQuery(row[0].ToString());
            }
            _sqlt.CommitTransaction();
        }

        public int CreateTables(string dbName, bool isTransact = true)
        {
            ClearDB();
            string sqlCmd = @"CREATE TABLE Schedules (
                    [ScheduleID] INTEGER PRIMARY KEY,
                    [Code] TEXT NOT NULL UNIQUE,
                    [Name] TEXT,
                    [Type] INTEGER NOT NULL
                    );";

            sqlCmd += @"CREATE TABLE Lessons (
                    [LessonID] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [ScheduleID] INTEGER NOT NULL,
                    [WeekNumber] INTEGER NOT NULL,
                    [DayOfWeek] INTEGER NOT NULL,
                    [LessonNumber] INTEGER NOT NULL,
                    [Subject] TEXT NOT NULL,
                    [Teacher] TEXT,
                    [Location] TEXT,
                    [StartTime] TEXT,
                    [EndTime] TEXT,
                    FOREIGN KEY ([ScheduleID]) REFERENCES Schedules([ScheduleID])
                    );";

            if (isTransact)
                _sqlt.BeginTransaction();

            ConnectionState previousConnectionState = ConnectionState.Closed;
            try
            {
                previousConnectionState = _sqlt.connect.State;
                if (_sqlt.connect.State == ConnectionState.Closed)
                {
                    _sqlt.connect.Open();
                }
                _sqlt.command = new SQLiteCommand(_sqlt.connect);
                _sqlt.command.CommandText = sqlCmd;
                _sqlt.command.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                _sqlt.SaveLog(1, string.Format("Ошибка при генерации таблиц новой базы данных: {0}!", error.Message));
                if (isTransact)
                    _sqlt.RollBackTransaction();
                return 0;
            }
            finally
            {
                if (previousConnectionState == ConnectionState.Closed)
                {
                    _sqlt.connect.Close();
                }
            }

            if (isTransact)
                _sqlt.CommitTransaction();
            return 1;
        }
        #endregion

        public int GetNextScheduleID()
        {
            DataTable dt = _sqlt.FetchByColumn(
                "Schedules",
                "MAX(ScheduleID) AS maxId",
                "",
                ""
            );

            int maxId = 0;
            if (dt.Rows.Count > 0 && dt.Rows[0]["maxId"].ToString() != "")
                maxId = int.Parse(dt.Rows[0]["maxId"].ToString());

            return maxId + 1;
        }

        public int CopySchedule(int oldScheduleID, string newCode, string newName)
        {
            int newID = GetNextScheduleID();

            int res = AddSchedule(newID, newCode, newName, 2);
            if (res == 0) return 0;

            DataTable dt = _sqlt.FetchByColumn(
                "Lessons",
                "WeekNumber, DayOfWeek, LessonNumber, Subject, Teacher, Location, Time",
                "ScheduleID = " + oldScheduleID,
                ""
            );

            foreach (DataRow row in dt.Rows)
            {
                string sql = @"INSERT INTO Lessons 
                       (ScheduleID, WeekNumber, DayOfWeek, LessonNumber, Subject, Teacher, Location, Time)
                       VALUES (" +
                               newID + ", " +
                               row["WeekNumber"] + ", " +
                               row["DayOfWeek"] + ", " +
                               row["LessonNumber"] + ", '" +
                               row["Subject"] + "', '" +
                               row["Teacher"] + "', '" +
                               row["Location"] + "', '" +
                               row["Time"] + "');";

                _sqlt.ExecuteNonQuery(sql);
            }

            return newID;
        }
        
        public int DeleteSchedule(int scheduleID)
        {
            try
            {
                _sqlt.BeginTransaction();

                _sqlt.ExecuteNonQuery("DELETE FROM Lessons WHERE ScheduleID = " + scheduleID);
                _sqlt.ExecuteNonQuery("DELETE FROM Schedules WHERE ScheduleID = " + scheduleID);

                _sqlt.CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                _sqlt.RollBackTransaction();
                SaveLog("Ошибка DeleteSchedule: " + ex.Message);
                return 0;
            }
        }

        public int GetSchedulesCount()
        {
            DataTable dt = _sqlt.FetchByColumn("Schedules", "COUNT(*) AS cnt", "", "");
            return int.Parse(dt.Rows[0]["cnt"].ToString());
        }

        #region Добавление расписания

        public int AddSchedule(int id, string code, string name, int type)
        {
            string sql = @"INSERT INTO Schedules ([ScheduleID], [Code], [Name], [Type])
                   VALUES (" + id + ", '" + code + "', '" + name + "', " + type + ");";

            try
            {
                _sqlt.ExecuteNonQuery(sql);
                return 1;
            }
            catch (Exception ex)
            {
                SaveLog("Ошибка AddSchedule: " + ex.Message);
                return 0;
            }
        }

        public int AddLesson(int scheduleID, int week, int day, int lessonNumber,
                             string subject, string teacher, string location, string startTime, string endTime)
        {
            string sql = @"INSERT INTO Lessons ([ScheduleID], [WeekNumber], [DayOfWeek], [LessonNumber], [Subject], [Teacher], [Location], [StartTime], [EndTime])
                       VALUES (" + scheduleID + ", " + week + ", " + day + ", " + lessonNumber + ", '" + subject + "', '" + teacher + "', '" + location + "', '" + startTime + "', '" + endTime + "');";

            try
            {
                _sqlt.ExecuteNonQuery(sql);
                return 1;
            }
            catch (Exception ex)
            {
                SaveLog("Ошибка AddLesson: " + ex.Message);
                return 0;
            }
        }

        #endregion

        #region Получение расписания

        public ScheduleInfo getScheduleByCode(string code)
        {
            DataTable dt = _sqlt.FetchByColumn("Schedules", "ScheduleID, Type", "Code = '" + code + "'", "");

            if (dt.Rows.Count == 0)
                return null;

            ScheduleInfo info = new ScheduleInfo();
            info.ScheduleID = int.Parse(dt.Rows[0]["ScheduleID"].ToString());
            info.Type = int.Parse(dt.Rows[0]["Type"].ToString());

            return info;
        }

        public List<LessonInfo> getOneWeekLessons(int scheduleID)
        {
            DataTable dt = _sqlt.FetchByColumn("Lessons",
                "LessonID, WeekNumber, DayOfWeek, LessonNumber, Subject, Teacher, Location, Time",
                "ScheduleID = " + scheduleID + " AND WeekNumber = 0",
                "ORDER BY DayOfWeek, LessonNumber");

            List<LessonInfo> lessons = new List<LessonInfo>();
            foreach (DataRow row in dt.Rows)
            {
                LessonInfo li = new LessonInfo();
                li.LessonID = int.Parse(row["LessonID"].ToString());
                li.WeekNumber = int.Parse(row["WeekNumber"].ToString());
                li.DayOfWeek = int.Parse(row["DayOfWeek"].ToString());
                li.LessonNumber = int.Parse(row["LessonNumber"].ToString());
                li.Subject = row["Subject"].ToString();
                li.Teacher = row["Teacher"].ToString();
                li.Location = row["Location"].ToString();
                li.Time = row["Time"].ToString();
                lessons.Add(li);
            }

            return lessons;
        }

        public List<LessonInfo> getTwoWeekLessons(int scheduleID, int weekNumber)
        {
            DataTable dt = _sqlt.FetchByColumn("Lessons",
                "LessonID, WeekNumber, DayOfWeek, LessonNumber, Subject, Teacher, Location, Time",
                "ScheduleID = " + scheduleID + " AND WeekNumber = " + weekNumber,
                "ORDER BY DayOfWeek, LessonNumber");

            List<LessonInfo> lessons = new List<LessonInfo>();
            foreach (DataRow row in dt.Rows)
            {
                LessonInfo li = new LessonInfo();
                li.LessonID = int.Parse(row["LessonID"].ToString());
                li.WeekNumber = int.Parse(row["WeekNumber"].ToString());
                li.DayOfWeek = int.Parse(row["DayOfWeek"].ToString());
                li.LessonNumber = int.Parse(row["LessonNumber"].ToString());
                li.Subject = row["Subject"].ToString();
                li.Teacher = row["Teacher"].ToString();
                li.Location = row["Location"].ToString();
                li.Time = row["Time"].ToString();
                lessons.Add(li);
            }

            return lessons;
        }

        #endregion
    }

    #region Классы для хранения информации

    public class ScheduleInfo
    {
        public int ScheduleID;
        public int Type;
    }

    public class LessonInfo
    {
        public int LessonID;
        public int WeekNumber;
        public int DayOfWeek;
        public int LessonNumber;
        public string Subject;
        public string Teacher;
        public string Location;
        public string Time;
    }

    #endregion
}
