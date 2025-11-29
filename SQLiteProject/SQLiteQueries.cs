using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            // ClearDB();

            string sqlCmd = @"CREATE TABLE IF NOT EXISTS Users (
            [UserID] INTEGER PRIMARY KEY AUTOINCREMENT,
            [PhoneNumber] TEXT NOT NULL UNIQUE
            );";

            sqlCmd += @"CREATE TABLE IF NOT EXISTS Schedules (
            [ScheduleID] INTEGER PRIMARY KEY,
            [Code] TEXT NOT NULL UNIQUE,
            [Name] TEXT,
            [Type] INTEGER NOT NULL
            );";

            sqlCmd = @"CREATE TABLE Schedules (
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

            sqlCmd += @"CREATE TABLE LessonOverrides (
                    [OverrideID] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [LessonID] INTEGER NOT NULL,
                    [OverrideDate] TEXT NOT NULL,
                    [IsActive] INTEGER NOT NULL,
                    [NewStartTime] TEXT,
                    [NewEndTime] TEXT,
                    [NewLocation] TEXT,
                    FOREIGN KEY ([LessonID]) REFERENCES Lessons([LessonID])
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

        public int CopySchedule(int oldScheduleID, int newScheduleID)
        {
            DataTable sched = _sqlt.FetchByColumn("Schedules", "*", "ScheduleID = " + oldScheduleID, "");
            if (sched.Rows.Count == 0) return 0;
            DataRow s = sched.Rows[0];

            int type = Convert.ToInt32(s["Type"]);
            string name = s["Name"].ToString();

            List<string> listSchedules = new List<string>()
            {
                $"{newScheduleID};0000;{name};{type}"
            };

            int errSched = AddSchedules(listSchedules);
            if (errSched > 0) return 0;

            DataTable lessons = _sqlt.FetchByColumn("Lessons", "*", "ScheduleID = " + oldScheduleID, "");

            List<string> listLessons = new List<string>();
            foreach (DataRow row in lessons.Rows)
            {
                string subject = row["Subject"] == DBNull.Value ? "" : row["Subject"].ToString();
                string teacher = row["Teacher"] == DBNull.Value ? "" : row["Teacher"].ToString();
                string location = row["Location"] == DBNull.Value ? "" : row["Location"].ToString();
                string startTime = row["StartTime"] == DBNull.Value ? "" : row["StartTime"].ToString();
                string endTime = row["EndTime"] == DBNull.Value ? "" : row["EndTime"].ToString();

                string lessonStr = $"{newScheduleID};{row["WeekNumber"]};{row["DayOfWeek"]};{row["LessonNumber"]};{subject};{teacher};{location};{startTime};{endTime}";
                listLessons.Add(lessonStr);
            }

            int errLessons = AddLessons(listLessons);
            if (errLessons > 0) return 0;

            return newScheduleID;
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

        public LessonInfo GetLessonById(int lessonId)
        {
            DataTable dt = _sqlt.FetchByColumn(
                "Lessons",
                "LessonID, WeekNumber, DayOfWeek, LessonNumber, Subject, Teacher, Location, StartTime, EndTime",
                "LessonID = " + lessonId,
                ""
            );

            if (dt.Rows.Count == 0)
                return null;

            DataRow row = dt.Rows[0];

            LessonInfo li = new LessonInfo();
            li.LessonID = int.Parse(row["LessonID"].ToString());
            li.WeekNumber = int.Parse(row["WeekNumber"].ToString());
            li.DayOfWeek = int.Parse(row["DayOfWeek"].ToString());
            li.LessonNumber = int.Parse(row["LessonNumber"].ToString());
            li.Subject = row["Subject"].ToString();
            li.Teacher = row["Teacher"].ToString();
            li.Location = row["Location"].ToString();

            // Тут у тебя StartTime / EndTime в базе, а в классах LessonInfo — поле Time
            li.Time = $"{row["StartTime"]}-{row["EndTime"]}";

            return li;
        }

        public int AddLessonOverrides(List<string> listOverrides)
        {
            if (listOverrides == null || listOverrides.Count == 0) return 0;

            ParametersCollection paramss = new ParametersCollection();
            int cntErr = 0;

            foreach (var item in listOverrides)
            {
                // Формат: "LessonID;OverrideDate;IsActive;NewStartTime;NewEndTime;NewLocation"
                string[] arr = item.Split(';');
                paramss.Clear();
                paramss.Add("LessonID", arr[0], System.Data.DbType.Int32);
                paramss.Add("OverrideDate", arr[1], System.Data.DbType.String);
                paramss.Add("IsActive", arr[2], System.Data.DbType.Int32);
                paramss.Add("NewStartTime", string.IsNullOrEmpty(arr[3]) ? null : arr[3], System.Data.DbType.String);
                paramss.Add("NewEndTime", string.IsNullOrEmpty(arr[4]) ? null : arr[4], System.Data.DbType.String);
                paramss.Add("NewLocation", string.IsNullOrEmpty(arr[5]) ? null : arr[5], System.Data.DbType.String);

                if (_sqlt.Insert("LessonOverrides", paramss) == 0) cntErr++;
            }

            return cntErr;
        }

        public int DeleteLessonForever(int lessonId)
        {
            try
            {
                ParametersCollection paramss = new ParametersCollection();
                paramss.Add("LessonID", lessonId, System.Data.DbType.Int32);

                _sqlt.Delete("Lessons", "LessonID=@LessonID", paramss);

                // Если исключений не было — считаем удаление успешным
                return 1;
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка DeleteLessonForever: {ex.Message}");
                return 0;
            }

        }

        public List<LessonInfo> GetLessonsForDate(DateTime date)
        {
            int scheduleID = 1; // всегда работаем с 1

            // День недели (1–7)
            int day = (int)date.DayOfWeek;
            if (day == 0) day = 7;

            // Получаем базовые уроки для этого дня
            DataTable dtBase = _sqlt.FetchByColumn(
                "Lessons",
                "LessonID, ScheduleID, WeekNumber, DayOfWeek, LessonNumber, Subject, Teacher, Location, StartTime, EndTime",
                $"ScheduleID = {scheduleID} AND DayOfWeek = {day}",
                "ORDER BY LessonNumber"
            );

            List<LessonInfo> baseLessons = new List<LessonInfo>();
            foreach (DataRow row in dtBase.Rows)
            {
                baseLessons.Add(new LessonInfo()
                {
                    LessonID = Convert.ToInt32(row["LessonID"]),
                    WeekNumber = Convert.ToInt32(row["WeekNumber"]),
                    DayOfWeek = Convert.ToInt32(row["DayOfWeek"]),
                    LessonNumber = Convert.ToInt32(row["LessonNumber"]),
                    Subject = row["Subject"].ToString(),
                    Teacher = row["Teacher"].ToString(),
                    Location = row["Location"].ToString(),
                    StartTime = row["StartTime"].ToString(),
                    EndTime = row["EndTime"].ToString(),
                    Time = $"{row["StartTime"]}-{row["EndTime"]}"
                });
            }

            // Берём все изменения на эту дату
            string dateStr = date.ToString("yyyy-MM-dd");
            DataTable dtOverrides = _sqlt.FetchByColumn(
                "LessonOverrides",
                "*",
                $"OverrideDate = '{dateStr}'",
                ""
            );

            // Копируем базовые уроки
            List<LessonInfo> finalList = new List<LessonInfo>(baseLessons);

            // Список для отменённых уроков
            List<int> canceledIds = new List<int>();

            foreach (DataRow row in dtOverrides.Rows)
            {
                int lessonId = Convert.ToInt32(row["LessonID"]);
                int isActive = Convert.ToInt32(row["IsActive"]);
                string newStart = row["NewStartTime"]?.ToString();
                string newEnd = row["NewEndTime"]?.ToString();
                string newLoc = row["NewLocation"]?.ToString();

                // Получаем ScheduleID урока из таблицы Lessons
                DataTable lessonRowDt = _sqlt.FetchByColumn("Lessons", "*", $"LessonID = {lessonId}", "");
                if (lessonRowDt.Rows.Count == 0) continue; // урок не найден
                DataRow lessonRow = lessonRowDt.Rows[0];
                int lessonScheduleID = Convert.ToInt32(lessonRow["ScheduleID"]);
                if (lessonScheduleID != scheduleID) continue; // не наше расписание — пропускаем

                if (isActive == 0)
                {
                    // отмена: добавляем в список для удаления
                    canceledIds.Add(lessonId);
                    continue;
                }

                // перенос/изменение
                LessonInfo baseLesson = finalList.FirstOrDefault(x => x.LessonID == lessonId);

                if (baseLesson != null)
                {
                    // меняем существующую пару
                    baseLesson.StartTime = string.IsNullOrEmpty(newStart) ? baseLesson.StartTime : newStart;
                    baseLesson.EndTime = string.IsNullOrEmpty(newEnd) ? baseLesson.EndTime : newEnd;
                    baseLesson.Location = string.IsNullOrEmpty(newLoc) ? baseLesson.Location : newLoc;
                    baseLesson.Time = $"{baseLesson.StartTime}-{baseLesson.EndTime}";
                }
                else
                {
                    // пары не было в расписании на этот день → добавляем новую
                    LessonInfo newLesson = new LessonInfo()
                    {
                        LessonID = lessonId,
                        WeekNumber = Convert.ToInt32(lessonRow["WeekNumber"]),
                        DayOfWeek = Convert.ToInt32(lessonRow["DayOfWeek"]),
                        LessonNumber = Convert.ToInt32(lessonRow["LessonNumber"]),
                        Subject = lessonRow["Subject"].ToString(),
                        Teacher = lessonRow["Teacher"].ToString(),
                        Location = string.IsNullOrEmpty(newLoc) ? lessonRow["Location"].ToString() : newLoc,
                        StartTime = string.IsNullOrEmpty(newStart) ? lessonRow["StartTime"].ToString() : newStart,
                        EndTime = string.IsNullOrEmpty(newEnd) ? lessonRow["EndTime"].ToString() : newEnd
                    };
                    newLesson.Time = $"{newLesson.StartTime}-{newLesson.EndTime}";

                    finalList.Add(newLesson);
                }
            }

            // Удаляем отменённые пары
            finalList.RemoveAll(x => canceledIds.Contains(x.LessonID));

            // Сортируем по StartTime
            finalList = finalList.OrderBy(x => TimeSpan.Parse(x.StartTime)).ToList();

            return finalList;
        }


        #region Добавление расписания
        public int AddSchedules(List<string> listSchedules)
        {
            if (listSchedules == null || listSchedules.Count == 0) return 0;

            ParametersCollection paramss = new ParametersCollection();
            int cntErr = 0;

            foreach (var sched in listSchedules)
            {
                string[] arr = sched.Split(';');
                paramss.Clear();
                paramss.Add("ScheduleID", arr[0], System.Data.DbType.Int32);
                paramss.Add("Code", arr[1], System.Data.DbType.String);
                paramss.Add("Name", arr[2], System.Data.DbType.String);
                paramss.Add("Type", arr[3], System.Data.DbType.Int32);

                if (_sqlt.Insert("Schedules", paramss) == 0) cntErr++;
            }

            return cntErr;
        }

        public int AddLessons(List<string> listLessons)
        {
            if (listLessons == null || listLessons.Count == 0) return 0;

            ParametersCollection paramss = new ParametersCollection();
            int cntErr = 0;

            foreach (var lesson in listLessons)
            {
                string[] arr = lesson.Split(';');
                paramss.Clear();
                paramss.Add("ScheduleID", arr[0], System.Data.DbType.Int32);
                paramss.Add("WeekNumber", arr[1], System.Data.DbType.Int32);
                paramss.Add("DayOfWeek", arr[2], System.Data.DbType.Int32);
                paramss.Add("LessonNumber", arr[3], System.Data.DbType.Int32);
                paramss.Add("Subject", arr[4], System.Data.DbType.String);
                paramss.Add("Teacher", arr[5], System.Data.DbType.String);
                paramss.Add("Location", arr[6], System.Data.DbType.String);
                paramss.Add("StartTime", arr[7], System.Data.DbType.String);
                paramss.Add("EndTime", arr[8], System.Data.DbType.String);

                if (_sqlt.Insert("Lessons", paramss) == 0) cntErr++;
            }

            return cntErr; // количество ошибок
        }
        #endregion
        #region Работа с пользователями

        public void CreateUsersTable()
        {
            try
            {
                string createTableQuery = @"
            CREATE TABLE IF NOT EXISTS Users (
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                PhoneNumber TEXT NOT NULL UNIQUE
            )";

                _sqlt.ExecuteNonQuery(createTableQuery);
            }
            catch (Exception ex)
            {
                SaveLog("Ошибка CreateUsersTable: " + ex.Message);
            }
        }

        public void AddUser(string phoneNumber)
        {
            try
            {
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Users (
                [UserID] INTEGER PRIMARY KEY AUTOINCREMENT,
                [PhoneNumber] TEXT NOT NULL UNIQUE
            )";
                _sqlt.ExecuteNonQuery(createTableQuery);

                string insertQuery = "INSERT OR IGNORE INTO Users (PhoneNumber) VALUES (@phoneNumber)";

                using (var connection = new SQLiteConnection(_sqlt.connect.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SQLiteCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                        command.ExecuteNonQuery(); // Просто выполняем, не проверяем результат
                    }
                }
            }
            catch (Exception ex)
            {
                SaveLog("Ошибка AddUser: " + ex.Message);
            }
        }

        public bool CheckUserExists(string phoneNumber)
        {
            try
            {
                DataTable dt = _sqlt.FetchByColumn(
                    "Users",
                    "COUNT(1) as UserCount",
                    "PhoneNumber = '" + phoneNumber + "'",
                    ""
                );

                // ДОБАВЬ ПРОВЕРКУ НА NULL:
                if (dt == null || dt.Rows.Count == 0)
                    return false;

                object userCount = dt.Rows[0]["UserCount"];
                if (userCount == null || userCount == DBNull.Value)
                    return false;

                return Convert.ToInt32(userCount) > 0;
            }
            catch (Exception ex)
            {
                SaveLog("Ошибка CheckUserExists: " + ex.Message);
                return false;
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
                "LessonID, WeekNumber, DayOfWeek, LessonNumber, Subject, Teacher, Location, StartTime, EndTime",
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

                string startTime = row["StartTime"].ToString();
                string endTime = row["EndTime"].ToString();
                li.Time = $"{startTime}-{endTime}"; 

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
        public string StartTime;
        public string EndTime;
    }

    #endregion
}
