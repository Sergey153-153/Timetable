using DatabaseLib;
using SQLiteProject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

        public int CreateTables_u(string dbName, bool isTransact = true)
        {
            // ClearDB();

            string sqlCmd = @"CREATE TABLE IF NOT EXISTS Users (
                    [UserID] INTEGER PRIMARY KEY AUTOINCREMENT,
                    [PhoneNumber] TEXT NOT NULL UNIQUE,
                    [DeviceToken] TEXT
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

        public int ClearLessonOverrides()
        {
            try
            {
                _sqlt.BeginTransaction();

                // Очистка всей таблицы LessonOverrides
                _sqlt.ExecuteNonQuery("DELETE FROM LessonOverrides;");

                _sqlt.CommitTransaction();
                return 1;
            }
            catch (Exception ex)
            {
                _sqlt.RollBackTransaction();
                SaveLog("Ошибка ClearLessonOverrides: " + ex.Message);
                return 0;
            }
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

        public DateTime SemesterStart = new DateTime(2025, 9, 1); // точка отсчёта

        public List<LessonInfo> GetLessonsForDate(DateTime date)
        {
            int scheduleID = 1;

            // Текущая учебная неделя: 1 или 2
            int weekNumber = ((int)((date - SemesterStart).TotalDays / 7) % 2) + 1;

            // День недели (1–7)
            int day = (int)date.DayOfWeek;
            if (day == 0) day = 7;

            bool isOneWeek = !_sqlt.FetchByColumn(
                "Lessons", "WeekNumber",
                $"ScheduleID = {scheduleID} AND WeekNumber > 0", ""
            ).AsEnumerable().Any();

            string weekFilter = isOneWeek
                ? ""                                                           // однонедельное → пары всегда показываем
                : $" AND (WeekNumber = 0 OR WeekNumber = {weekNumber})";        // двухнедельное

            DataTable dtBase = _sqlt.FetchByColumn(
                "Lessons",
                "LessonID, ScheduleID, WeekNumber, DayOfWeek, LessonNumber, Subject, Teacher, Location, StartTime, EndTime",
                $"ScheduleID = {scheduleID} AND DayOfWeek = {day}{weekFilter}",
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

            // Берём override
            string dateStr = date.ToString("yyyy-MM-dd");
            DataTable dtOverrides = _sqlt.FetchByColumn("LessonOverrides", "*", $"OverrideDate = '{dateStr}'", "");

            // Копируем базовые уроки
            List<LessonInfo> finalList = new List<LessonInfo>(baseLessons);

            List<int> canceledIds = new List<int>();

            foreach (DataRow row in dtOverrides.Rows)
            {
                int lessonId = Convert.ToInt32(row["LessonID"]);
                int isActive = Convert.ToInt32(row["IsActive"]);
                string newStart = row["NewStartTime"]?.ToString();
                string newEnd = row["NewEndTime"]?.ToString();
                string newLoc = row["NewLocation"]?.ToString();

                // Берём исходный урок
                DataTable lessonRowDt = _sqlt.FetchByColumn("Lessons", "*", $"LessonID = {lessonId}", "");
                if (lessonRowDt.Rows.Count == 0) continue;
                DataRow lessonRow = lessonRowDt.Rows[0];

                // Проверка расписания
                int lessonScheduleID = Convert.ToInt32(lessonRow["ScheduleID"]);
                if (lessonScheduleID != scheduleID) continue;

                if (isActive == 0)
                {
                    canceledIds.Add(lessonId);
                    continue;
                }

                LessonInfo baseLesson = finalList.FirstOrDefault(x => x.LessonID == lessonId);

                if (baseLesson != null)
                {
                    baseLesson.StartTime = string.IsNullOrEmpty(newStart) ? baseLesson.StartTime : newStart;
                    baseLesson.EndTime = string.IsNullOrEmpty(newEnd) ? baseLesson.EndTime : newEnd;
                    baseLesson.Location = string.IsNullOrEmpty(newLoc) ? baseLesson.Location : newLoc;
                    baseLesson.Time = $"{baseLesson.StartTime}-{baseLesson.EndTime}";
                }
                else
                {
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

            // Удаляем отменённые
            finalList.RemoveAll(x => canceledIds.Contains(x.LessonID));

            foreach (var les in finalList)
            {
                // Если расписание однонедельное → всегда 0
                if (isOneWeek)
                    les.WeekNumber = 0;
                else
                    les.WeekNumber = weekNumber; // 1 или 2
            }

            // Сортировка по времени
            finalList = finalList.OrderBy(x => TimeSpan.Parse(x.StartTime)).ToList();

            return finalList;
        }

        public List<TaskItem> GetTasksForLesson(int lessonId)
        {
            List<TaskItem> result = new List<TaskItem>();

            // 1) Берём предмет по LessonID
            DataTable dtLesson = _sqlt.FetchByColumn(
                "Lessons",
                "Subject",
                $"LessonID = {lessonId}",
                ""
            );

            if (dtLesson.Rows.Count == 0)
                return result; // урока нет → заданий нет

            string subjectName = dtLesson.Rows[0]["Subject"].ToString();

            // 2) Берём все задания по этому предмету (без фильтра по дате!)
            DataTable dtTasks = _sqlt.FetchByColumn(
                "Tasks",
                "TaskID, Title, Description, Deadline, Type, IsCompleted, SubjectName, FilePath",
                $"SubjectName = '{subjectName}'",
                "ORDER BY Deadline"
            );

            foreach (DataRow row in dtTasks.Rows)
            {
                result.Add(new TaskItem()
                {
                    Id = Convert.ToInt32(row["TaskID"]),
                    Title = row["Title"].ToString(),
                    Description = row["Description"].ToString(),
                    Deadline = DateTime.TryParse(row["Deadline"].ToString(), out var dt) ? dt : DateTime.MinValue,
                    Type = row["Type"].ToString(),
                    IsCompleted = Convert.ToInt32(row["IsCompleted"]) == 1,
                    SubjectName = row["SubjectName"].ToString(),
                    FilePath = row["FilePath"].ToString()
                });
            }

            return result;
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

        public void AddUser(string phoneNumber)
        {
            try
            {
                string createTableQuery = @"CREATE TABLE IF NOT EXISTS Users (
                [UserID] INTEGER PRIMARY KEY AUTOINCREMENT,
                [PhoneNumber] TEXT NOT NULL UNIQUE,
                [DeviceToken] TEXT
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
            DataTable dt = _sqlt.FetchByColumn("Schedules", "ScheduleID, Type, Code, Name", "Code = '" + code + "'", "");

            if (dt.Rows.Count == 0)
                return null;

            ScheduleInfo info = new ScheduleInfo();
            info.ScheduleID = int.Parse(dt.Rows[0]["ScheduleID"].ToString());
            info.Type = int.Parse(dt.Rows[0]["Type"].ToString());
            info.Code = dt.Rows[0]["Code"].ToString();
            info.Name = dt.Rows[0]["Name"]?.ToString() ?? "";

            return info;
        }

        public List<ScheduleInfo> GetAllSchedules()
        {
            List<ScheduleInfo> schedules = new List<ScheduleInfo>();

            try
            {
                DataTable dt = _sqlt.FetchByColumn(
                    "Schedules",
                    "ScheduleID, Code, Name, Type",
                    "",
                    "ORDER BY ScheduleID"
                );

                foreach (DataRow row in dt.Rows)
                {
                    ScheduleInfo info = new ScheduleInfo();
                    info.ScheduleID = int.Parse(row["ScheduleID"].ToString());
                    info.Code = row["Code"].ToString();
                    info.Name = row["Name"]?.ToString() ?? "";
                    info.Type = int.Parse(row["Type"].ToString());
                    schedules.Add(info);
                }
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка GetAllSchedules: {ex.Message}");
            }

            return schedules;
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

        #endregion

        #region Создание таблиц для задач

        // Исправленное название метода
        public int CreateTasksTables(string dbName, bool isTransact = true)
        {
            string sqlCmd = @"CREATE TABLE IF NOT EXISTS Subjects (
                                [SubjectID] INTEGER PRIMARY KEY AUTOINCREMENT,
                                [Name] TEXT NOT NULL UNIQUE,
                                [CreatedDate] TEXT DEFAULT CURRENT_TIMESTAMP
                            );

                            CREATE TABLE IF NOT EXISTS Tasks (
                                [TaskID] INTEGER PRIMARY KEY AUTOINCREMENT,
                                [Title] TEXT NOT NULL,
                                [Description] TEXT,
                                [Deadline] TEXT NOT NULL,
                                [Type] TEXT NOT NULL,
                                [SubjectName] TEXT NOT NULL,
                                [FilePath] TEXT,
                                [IsCompleted] INTEGER DEFAULT 0,
                                [CreatedDate] TEXT DEFAULT CURRENT_TIMESTAMP,
                                FOREIGN KEY (SubjectName) REFERENCES Subjects(Name)
                            );
                        ";

            if (isTransact)
                _sqlt.BeginTransaction();

            ConnectionState previousConnectionState = ConnectionState.Closed;
            try
            {
                previousConnectionState = _sqlt.connect.State;
                if (_sqlt.connect.State == ConnectionState.Closed)
                    _sqlt.connect.Open();

                _sqlt.command = new SQLiteCommand(_sqlt.connect)
                {
                    CommandText = sqlCmd
                };
                _sqlt.command.ExecuteNonQuery();
            }
            catch (Exception error)
            {
                _sqlt.SaveLog(1, $"Ошибка при генерации таблиц: {error.Message}");
                if (isTransact) _sqlt.RollBackTransaction();
                return 0;
            }
            finally
            {
                if (previousConnectionState == ConnectionState.Closed)
                    _sqlt.connect.Close();
            }

            if (isTransact) _sqlt.CommitTransaction();
            return 1;
        }

        public void AddDefaultSubjects()
        {
            try
            {
                // Удаляем все старые предметы
                _sqlt.Execute("DELETE FROM Subjects");

                // Берем уникальные предметы из таблицы Lessons для ScheduleID = 1
                DataTable dtLessons = _sqlt.Execute("SELECT DISTINCT [Subject] FROM Lessons WHERE ScheduleID = 1 AND [Subject] IS NOT NULL AND [Subject] != ''");

                List<string> subjectsToAdd = new List<string>();

                if (dtLessons != null && dtLessons.Rows.Count > 0)
                {
                    foreach (DataRow row in dtLessons.Rows)
                    {
                        string subject = row["Subject"].ToString().Trim();
                        if (!string.IsNullOrWhiteSpace(subject))
                            subjectsToAdd.Add(subject);
                    }
                }

                // Если Lessons пустые или null, добавляем дефолтные
                if (subjectsToAdd.Count == 0)
                {
                    subjectsToAdd.AddRange(new string[] {
                        "Математика", "Физика", "Химия", "История", "Литература",
                        "Программирование", "Английский язык", "Биология", "География"
                    });
                }

                // Добавление через ParametersCollection
                foreach (var subj in subjectsToAdd)
                {
                    var paramss = new ParametersCollection();
                    paramss.Add("Name", subj, DbType.String);
                    _sqlt.Insert("Subjects", paramss);
                }
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка добавления предметов по умолчанию: {ex.Message}");
            }
        }

        #endregion

        #region Методы для работы с задачами

        // 1. Добавление новой задачи
        public int AddTask(TaskItem task)
        {
            try
            {
                var parameters = new ParametersCollection();
                parameters.Add("Title", task.Title, DbType.String);
                parameters.Add("Description", task.Description, DbType.String);
                parameters.Add("Deadline", task.Deadline.ToString("yyyy-MM-dd HH:mm:ss"), DbType.String);
                parameters.Add("Type", task.Type, DbType.String);
                parameters.Add("SubjectName", task.SubjectName, DbType.String);
                parameters.Add("FilePath", task.FilePath, DbType.String);
                parameters.Add("IsCompleted", task.IsCompleted ? 1 : 0, DbType.Int32);

                int result = _sqlt.Insert("Tasks", parameters);
                if (result > 0)
                {
                    DataTable dt = _sqlt.Execute("SELECT last_insert_rowid() as TaskID");
                    if (dt.Rows.Count > 0)
                        return Convert.ToInt32(dt.Rows[0]["TaskID"]);
                }
                return 0;
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка AddTask: {ex.Message}");
                return 0;
            }
        }

        // Упрощенная версия AddTask для формы
        public int AddSimpleTask(string title, string description, DateTime deadline, string type, string subjectName, string filePath = null)
        {
            TaskItem task = new TaskItem
            {
                Title = title,
                Description = description,
                Deadline = deadline,
                Type = type,
                SubjectName = subjectName,
                FilePath = filePath,
                IsCompleted = false
            };

            return AddTask(task);
        }

        // 2. Получение всех задач
        public List<TaskItem> GetAllTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();
            try
            {
                DataTable dt = _sqlt.FetchByColumn("Tasks", "*", "", "ORDER BY Deadline ASC");
                foreach (DataRow row in dt.Rows)
                {
                    tasks.Add(new TaskItem
                    {
                        Id = Convert.ToInt32(row["TaskID"]),
                        Title = row["Title"].ToString(),
                        Description = row["Description"].ToString(),
                        Deadline = DateTime.Parse(row["Deadline"].ToString()),
                        Type = row["Type"].ToString(),
                        SubjectName = row["SubjectName"].ToString(),
                        FilePath = row["FilePath"].ToString(),
                        IsCompleted = Convert.ToBoolean(row["IsCompleted"])
                    });
                }
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка GetAllTasks: {ex.Message}");
            }
            return tasks;
        }

        // 3. Получение задач по фильтру (исправленная версия без параметров)
        public List<TaskItem> GetTasksByFilter(string subjectName = null, string type = null, bool? isCompleted = null)
        {
            List<TaskItem> tasks = new List<TaskItem>();

            try
            {
                string whereClause = "1=1";

                if (!string.IsNullOrEmpty(subjectName))
                {
                    whereClause += $" AND SubjectName = '{subjectName.Replace("'", "''")}'";
                }

                if (!string.IsNullOrEmpty(type))
                {
                    whereClause += $" AND Type = '{type.Replace("'", "''")}'";
                }

                if (isCompleted.HasValue)
                {
                    whereClause += $" AND IsCompleted = {(isCompleted.Value ? 1 : 0)}";
                }

                DataTable dt = _sqlt.FetchByColumn("Tasks", "*", whereClause, "ORDER BY Deadline ASC");

                foreach (DataRow row in dt.Rows)
                {
                    tasks.Add(new TaskItem
                    {
                        Id = Convert.ToInt32(row["TaskID"]),
                        Title = row["Title"].ToString(),
                        Description = row["Description"].ToString(),
                        Deadline = DateTime.Parse(row["Deadline"].ToString()),
                        Type = row["Type"].ToString(),
                        SubjectName = row["SubjectName"].ToString(),
                        FilePath = row["FilePath"].ToString(),
                        IsCompleted = Convert.ToBoolean(row["IsCompleted"])
                    });
                }
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка GetTasksByFilter: {ex.Message}");
            }

            return tasks;
        }

        // 4. Обновление задачи

        public bool ReplaceTask(TaskItem task)
        {
            try
            {
                // Сначала удаляем старую запись
                bool deleted = DeleteTask(task.Id);

                // Добавляем новую запись
                int newId = AddTask(task);

                // Обновляем ID объекта на новый
                task.Id = newId;

                return true;
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка ReplaceTask: {ex.Message}");
                return false;
            }
        }


        // 5. Удаление задачи
        public bool DeleteTask(int taskId)
        {
            try
            {
                var parameters = new ParametersCollection();
                parameters.Add("TaskID", taskId, DbType.Int32);
                int result = _sqlt.Delete("Tasks", "TaskID = @TaskID", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка DeleteTask: {ex.Message}");
                return false;
            }
        }

        // 6. Изменение статуса выполнения
        public bool ToggleTaskCompletion(int taskId, bool isCompleted)
        {
            try
            {
                var parameters = new ParametersCollection();
                parameters.Add("IsCompleted", isCompleted ? 1 : 0, DbType.Int32);
                parameters.Add("TaskID", taskId, DbType.Int32);

                string whereClause = "TaskID = @TaskID";
                int result = _sqlt.Update("Tasks", parameters, whereClause);

                return result > 0;
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка ToggleTaskCompletion: {ex.Message}");
                return false;
            }
        }

        // 7. Получение задачи по ID (исправленная без параметров)
        public TaskItem GetTaskById(int taskId)
        {
            try
            {
                DataTable dt = _sqlt.FetchByColumn(
                    "Tasks",
                    "*",
                    $"TaskID = {taskId}",
                    ""
                );

                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new TaskItem
                    {
                        Id = Convert.ToInt32(row["TaskID"]),
                        Title = row["Title"].ToString(),
                        Description = row["Description"].ToString(),
                        Deadline = DateTime.Parse(row["Deadline"].ToString()),
                        Type = row["Type"].ToString(),
                        SubjectName = row["SubjectName"].ToString(),
                        FilePath = row["FilePath"].ToString(),
                        IsCompleted = Convert.ToBoolean(row["IsCompleted"])
                    };
                }
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка GetTaskById: {ex.Message}");
            }

            return null;
        }

        #endregion

        #region Методы для работы с предметами

        // 1. Получение всех предметов
        public List<SubjectItem> GetAllSubjects()
        {
            List<SubjectItem> subjects = new List<SubjectItem>();
            try
            {
                DataTable dt = _sqlt.FetchByColumn("Subjects", "*", "", "ORDER BY Name ASC");
                foreach (DataRow row in dt.Rows)
                {
                    subjects.Add(new SubjectItem
                    {
                        Id = Convert.ToInt32(row["SubjectID"]),
                        Name = row["Name"].ToString()
                    });
                }

                if (subjects.Count == 0) // базовые предметы
                {
                    string[] defaultSubjects = { "Математика", "Физика", "Химия", "История", "Литература" };
                    subjects.AddRange(defaultSubjects.Select(s => new SubjectItem { Name = s }));
                }
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка GetAllSubjects: {ex.Message}");
            }
            return subjects;
        }

        // Добавление нового предмета
        public bool AddSubject(string subjectName)
        {
            try
            {
                var parameters = new ParametersCollection();
                parameters.Add("Name", subjectName, DbType.String);
                int result = _sqlt.Insert("Subjects", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка AddSubject: {ex.Message}");
                return false;
            }
        }

        // Проверка существования предмета
        public bool SubjectExists(string subjectName)
        {
            try
            {
                DataTable dt = _sqlt.FetchByColumn("Subjects", "COUNT(*) as Count", $"Name = '{subjectName.Replace("'", "''")}'", "");
                return dt.Rows.Count > 0 && Convert.ToInt32(dt.Rows[0]["Count"]) > 0;
            }
            catch (Exception ex)
            {
                SaveLog($"Ошибка SubjectExists: {ex.Message}");
                return false;
            }
        }

        #endregion

    }

    #region Классы для хранения информации

    public class ScheduleInfo
    {
        public int ScheduleID;
        public int Type;
        public string Code;
        public string Name;
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