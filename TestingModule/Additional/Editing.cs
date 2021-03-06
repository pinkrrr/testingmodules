﻿using System;
using System.Linq;
using TestingModule.Models;

namespace TestingModule.Additional
{
    public class Editing : IDisposable
    {
        private readonly testingDbEntities _db;

        public Editing(testingDbEntities context)
        {
            _db = context;
        }
        public void EditDiscipline(int disciplineId, string name, int? lectorId)
        {
            var disc = _db.Disciplines.FirstOrDefault(t => t.Id == disciplineId);
            if (lectorId != null)
            {
                if (_db.LectorDisciplines.Any(t => t.DisciplineId == disciplineId))
                {
                    var connect = _db.LectorDisciplines.FirstOrDefault(t => t.DisciplineId == disciplineId);
                    if (connect != null) connect.LectorId = Convert.ToInt32(lectorId);
                    _db.SaveChanges();
                }
                else
                {
                    var lecturesTable = _db.Set<LectorDiscipline>();
                    lecturesTable.Add(new LectorDiscipline() { LectorId = Convert.ToInt32(lectorId), DisciplineId = disciplineId });
                }
            }
            if (disc != null) disc.Name = name;
            _db.SaveChanges();
        }
        public void EditLecture(int lectureId, string name, int disciplineId)
        {
            var lct = _db.Lectures.FirstOrDefault(t => t.Id == lectureId);
            if (disciplineId != _db.Lectures.FirstOrDefault(t => t.Id == lectureId).DisciplineId && disciplineId != 0)
            {
                lct.DisciplineId = disciplineId;
                var modules = _db.Modules.Where(t => t.LectureId == lectureId);
                var questions = _db.Questions.Where(t => t.LectureId == lectureId);
                foreach (var module in modules)
                {
                    module.DisciplineId = disciplineId;
                }
                foreach (var question in questions)
                {
                    question.DisciplineId = disciplineId;
                }
            }
            lct.Name = name;
            _db.SaveChanges();
        }
        public void EditModule(int moduleId, string name, int lectureId, int minutes, string description)
        {
            var mdl = _db.Modules.FirstOrDefault(t => t.Id == moduleId);
            if (lectureId != _db.Modules.FirstOrDefault(t => t.Id == moduleId).LectureId && lectureId != 0)
            {
                mdl.LectureId = lectureId;
                var questions = _db.Questions.Where(t => t.ModuleId == moduleId);
                foreach (var question in questions)
                {
                    question.LectureId = lectureId;
                }
            }
            mdl.Name = name;
            mdl.Description = description;
            if (minutes != 0)
            {
                mdl.MinutesToPass = minutes;
            }
            _db.SaveChanges();
        }

        public void EditMaterial(int moduleId, string description)
        {
            var mdl = _db.Modules.FirstOrDefault(t => t.Id == moduleId);
            mdl.Description = description;
            _db.SaveChanges();
        }
        public void EditQuestion(int questionId, string text, int moduleId, int questionType)
        {
            var qs = _db.Questions.FirstOrDefault(t => t.Id == questionId);
            if (moduleId != _db.Questions.FirstOrDefault(t => t.Id == questionId).ModuleId && moduleId != 0)
            {
                qs.ModuleId = moduleId;
            }
            qs.QuestionType = questionType;
            qs.Text = text;
            _db.SaveChanges();
        }
        public void EditAnswer(int? answerId, string text, bool? isCorrect, int correctAnswerId)
        {
            if (correctAnswerId != 0)
            {
                _db.Answers.FirstOrDefault(t => t.Id == correctAnswerId).IsCorrect = true;
            }
            if (answerId == correctAnswerId)
            {
                isCorrect = true;
            }
            else
            {
                isCorrect = false;
            }
            var anw = _db.Answers.FirstOrDefault(t => t.Id == answerId);
            anw.Text = text;
            anw.IsCorrect = isCorrect;
            _db.SaveChanges();
        }

        public void EditSpeciality(int specialityId, string name)
        {
            var spc = _db.Specialities.FirstOrDefault(t => t.Id == specialityId);
            spc.Name = name;
            _db.SaveChanges();
        }
        public void EditGroup(int groupId, string name, int specialityId)
        {
            var grp = _db.Groups.FirstOrDefault(t => t.Id == groupId);
            if (specialityId != _db.Groups.FirstOrDefault(t => t.Id == groupId).SpecialityId && specialityId != 0)
            {
                grp.SpecialityId = specialityId;
                var students = _db.Students.Where(t => t.GroupId == groupId);
                foreach (var student in students)
                {
                    student.SpecialityId = specialityId;
                }
            }
            grp.Name = name;
            _db.SaveChanges();
        }
        public void EditStudent(int studentId, string name, string surname, String username, String pass, int groupId)
        {
            var std = _db.Students.FirstOrDefault(t => t.Id == studentId);
            var ac = _db.Accounts.FirstOrDefault(t => t.Id == std.AccountId);
            if (groupId != _db.Students.FirstOrDefault(t => t.Id == studentId).GroupId && groupId != 0)
            {
                std.GroupId = groupId;
            }
            std.Name = name;
            std.Surname = surname;
            if (username != null && pass != null && username != "" && pass != "")
            {
                ac.Login = username;
                ac.Password = pass;
            }
            _db.SaveChanges();
        }
        public void EditLector(int lectorId, string name, string surname, string username, string pass)
        {
            var lct = _db.Lectors.FirstOrDefault(t => t.Id == lectorId);
            var ac = _db.Accounts.FirstOrDefault(t => t.Id == lct.AccountId);
            lct.Name = name;
            lct.Surname = surname;
            ac.Login = username;
            ac.Password = pass;
            _db.SaveChanges();
        }
        public void EditExeption(int exeptionId)
        {
            var exl = _db.ExeptionLogs.FirstOrDefault(t => t.Id == exeptionId);
            exl.Resolved = true;
            _db.SaveChanges();
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}