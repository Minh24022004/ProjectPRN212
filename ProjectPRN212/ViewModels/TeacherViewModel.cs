using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectPRN212.Models;
using Microsoft.EntityFrameworkCore;

namespace ProjectPRN212.ViewModels
{
    public class TeacherViewModel : ObservableObject
    {
        private readonly SafeDrivingCertificateDbContext _context;
        private Course _newCourse;
        private Course _selectedCourse;
        private Registration _selectedRegistration;
        private Result _newResult;
        private Exam _selectedExam;
        private User _selectedUser;
        private string _notificationMessage;

        public ObservableCollection<Course> Courses { get; set; }
        public ObservableCollection<Registration> Registrations { get; set; }
        public ObservableCollection<Result> Results { get; set; }
        public ObservableCollection<Exam> Exams { get; set; }
        public ObservableCollection<User> Users { get; set; }

        public Course NewCourse
        {
            get => _newCourse;
            set => SetProperty(ref _newCourse, value);
        }

        public Course SelectedCourse
        {
            get => _selectedCourse;
            set
            {
                SetProperty(ref _selectedCourse, value);
                if (value != null)
                {
                    NewCourse = new Course
                    {
                        CourseId = value.CourseId,
                        CourseName = value.CourseName,
                        StartDate = value.StartDate,
                        EndDate = value.EndDate,
                        TeacherId = value.TeacherId
                    };
                }
            }
        }

        public Registration SelectedRegistration
        {
            get => _selectedRegistration;
            set => SetProperty(ref _selectedRegistration, value);
        }

        public Result NewResult
        {
            get => _newResult;
            set => SetProperty(ref _newResult, value);
        }

        public Exam SelectedExam
        {
            get => _selectedExam;
            set => SetProperty(ref _selectedExam, value);
        }

        public User SelectedUser
        {
            get => _selectedUser;
            set => SetProperty(ref _selectedUser, value);
        }

        public string NotificationMessage
        {
            get => _notificationMessage;
            set => SetProperty(ref _notificationMessage, value);
        }

        public RelayCommand AddCourseCommand { get; }
        public RelayCommand UpdateCourseCommand { get; }
        public RelayCommand<Course> DeleteCourseCommand { get; }
        public RelayCommand<Registration> ApproveRegistrationCommand { get; }
        public RelayCommand<Registration> RejectRegistrationCommand { get; }
        public RelayCommand SubmitResultCommand { get; }
        public RelayCommand SendNotificationCommand { get; }

        public TeacherViewModel()
        {
            _context = new SafeDrivingCertificateDbContext(new DbContextOptionsBuilder<SafeDrivingCertificateDbContext>()
                .UseSqlServer("Server= (local) ;uid=sa;password=123;database=SafeDrivingDB;Encrypt=True;TrustServerCertificate=True;").Options);
            Courses = new ObservableCollection<Course>();
            Registrations = new ObservableCollection<Registration>();
            Results = new ObservableCollection<Result>();
            Exams = new ObservableCollection<Exam>();
            Users = new ObservableCollection<User>();
            NewCourse = new Course();
            NewResult = new Result();

            AddCourseCommand = new RelayCommand(AddCourse);
            UpdateCourseCommand = new RelayCommand(UpdateCourse);
            DeleteCourseCommand = new RelayCommand<Course>(DeleteCourse);
            ApproveRegistrationCommand = new RelayCommand<Registration>(ApproveRegistration);
            RejectRegistrationCommand = new RelayCommand<Registration>(RejectRegistration);
            SubmitResultCommand = new RelayCommand(SubmitResult);
            SendNotificationCommand = new RelayCommand(SendNotification);

            LoadData();
        }

        private void LoadData()
        {
            Courses.Clear();
            Registrations.Clear();
            Results.Clear();
            Exams.Clear();
            Users.Clear();

            var courses = _context.Courses.Include(c => c.Teacher).ToList();
            foreach (var course in courses)
                Courses.Add(course);

            var registrations = _context.Registrations.Include(r => r.User).Include(r => r.Course).ToList();
            foreach (var reg in registrations)
                Registrations.Add(reg);

            var results = _context.Results.Include(r => r.User).Include(r => r.Exam).ToList();
            foreach (var result in results)
                Results.Add(result);

            var exams = _context.Exams.ToList();
            foreach (var exam in exams)
                Exams.Add(exam);

            var users = _context.Users.Where(u => u.Role == "Student").ToList();
            foreach (var user in users)
                Users.Add(user);
        }

        private void AddCourse()
        {
            NewCourse.TeacherId = 1; // Giả định TeacherID, thay bằng ID thực tế
            _context.Courses.Add(NewCourse);
            _context.SaveChanges();
            Courses.Add(NewCourse);
            NewCourse = new Course();
            OnPropertyChanged(nameof(NewCourse));

        }

        private void UpdateCourse()
        {
            if (SelectedCourse != null)
            {
                var course = _context.Courses.Find(SelectedCourse.CourseId);
                if (course != null)
                {
                    course.CourseName = NewCourse.CourseName;
                    course.StartDate = NewCourse.StartDate;
                    course.EndDate = NewCourse.EndDate;
                    _context.SaveChanges();
                    LoadData();
                }
            }
        }

        private void DeleteCourse(Course course)
        {
            if (course != null)
            {
                _context.Courses.Remove(course);
                _context.SaveChanges();
                Courses.Remove(course);
            }
        }

        private void ApproveRegistration(Registration registration)
        {
            if (registration != null)
            {
                registration.Status = "Approved";
                _context.SaveChanges();
                LoadData();
                SendAutoNotification(registration.UserId, $"Your registration for {registration.Course.CourseName} has been approved.");
            }
        }

        private void RejectRegistration(Registration registration)
        {
            if (registration != null)
            {
                registration.Status = "Rejected";
                _context.SaveChanges();
                LoadData();
                SendAutoNotification(registration.UserId, $"Your registration for {registration.Course.CourseName} has been rejected.");
            }
        }

        private void SubmitResult()
        {
            if (SelectedExam != null && SelectedRegistration != null)
            {
                NewResult.ExamId = SelectedExam.ExamId;
                NewResult.UserId = SelectedRegistration.UserId;
                _context.Results.Add(NewResult);
                _context.SaveChanges();
                Results.Add(NewResult);
                SendAutoNotification(NewResult.UserId, $"Your exam result: Score {NewResult.Score}, Pass: {NewResult.PassStatus}.");
                NewResult = new Result();
                OnPropertyChanged(nameof(NewCourse));

            }
        }

        private void SendNotification()
        {
            if (SelectedUser != null && !string.IsNullOrEmpty(NotificationMessage))
            {
                var notification = new Notification
                {
                    UserID = SelectedUser.UserId,
                    Message = NotificationMessage,
                    CreatedAt = DateTime.Now,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
                _context.SaveChanges();
                NotificationMessage = string.Empty;
                OnPropertyChanged(nameof(NewCourse));

            }
        }

        private void SendAutoNotification(int userId, string message)
        {
            var notification = new Notification
            {
                UserID = userId,
                Message = message,
                CreatedAt = DateTime.Now,
                IsRead = false
            };
            _context.Notifications.Add(notification);
            _context.SaveChanges();
        }
    }
}
