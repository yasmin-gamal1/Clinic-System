using System;
using System.Collections.Generic;

public enum WorkingDays { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }

public class Account
{
    public int AccountID { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public bool Online { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public DateTime LastLoggedIn { get; set; }

    public virtual bool Login(string username, string password)
    {
        if (Username == username && Password == password)
        {
            Online = true;
            LastLoggedIn = DateTime.Now;
            return true;
        }
        return false;
    }

    public virtual bool Logout()
    {
        Online = false;
        return true;
    }
}

public class Doctor : Account
{
    public bool FullAuthorization { get; set; } = true;
    public List<WorkingDays> Schedule { get; set; } = new List<WorkingDays>();
    public List<Assistant> Assistants { get; set; } = new List<Assistant>();
    public List<Patient> WaitingList { get; set; } = new List<Patient>();

    public void AddAssistant(Assistant assistant)
    {
        Assistants.Add(assistant);
    }

    public void GetSchedule()
    {
        Console.WriteLine("Doctor's working days: ");
        foreach (var day in Schedule)
        {
            Console.WriteLine(day);
        }
    }

    public bool AddToWaitingList(Patient patient)
    {
        WaitingList.Add(patient);
        return true;
    }

    public void AddNewPatient(Patient patient)
    {
        WaitingList.Add(patient);
        Console.WriteLine($"{patient.PatientName} added to waiting list.");
    }

    public bool IsAvailable(DateTime date, TimeSpan time)
    {
        // Check if the doctor is available at the given date and time.
        // This would be based on actual appointment checks
        return true; // For now assuming the doctor is available.
    }

    public bool IsTimeUntaken(DateTime date, TimeSpan time)
    {
        // Check if the doctor already has an appointment at the given time
        return true; // Assuming time is available.
    }
}

public class Assistant : Account
{
    public bool FullAuthorization { get; set; } = false;
    public List<Patient> Patients { get; set; } = new List<Patient>();

    public void AddNewPatient(Patient patient)
    {
        Patients.Add(patient);
    }
}

public class Patient
{
    public string PatientName { get; set; }
    public int Age { get; set; }
    public string Address { get; set; }
    public string History { get; set; }
    public string Gender { get; set; }
}

public class Appointment
{
    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }
    public Patient Patient { get; set; }
    public Assistant Assistant { get; set; }
    public Doctor Doctor { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; }

    public Appointment(DateTime date, TimeSpan time, Patient patient, Assistant assistant, Doctor doctor)
    {
        Date = date;
        Time = time;
        Patient = patient;
        Assistant = assistant;
        Doctor = doctor;
    }
}

public class DB
{
    public Dictionary<string, Appointment> AppointmentList { get; set; } = new Dictionary<string, Appointment>();
    public Dictionary<int, Patient> PatientList { get; set; } = new Dictionary<int, Patient>();
    public Dictionary<int, Assistant> AssistantList { get; set; } = new Dictionary<int, Assistant>();

    public bool Insert(string key, Appointment appointment)
    {
        if (!AppointmentList.ContainsKey(key))
        {
            AppointmentList.Add(key, appointment);
            return true;
        }
        return false;
    }

    public bool Update(string key, Appointment appointment)
    {
        if (AppointmentList.ContainsKey(key))
        {
            AppointmentList[key] = appointment;
            return true;
        }
        return false;
    }

    public bool Delete(string key)
    {
        if (AppointmentList.ContainsKey(key))
        {
            AppointmentList.Remove(key);
            return true;
        }
        return false;
    }

    public List<Appointment> Select()
    {
        List<Appointment> appointments = new List<Appointment>(AppointmentList.Values);
        return appointments;
    }

    public void AddAppointment(DateTime date, TimeSpan time, Patient patient, Doctor doctor, Assistant assistant)
    {
        string key = $"{date.ToShortDateString()} {time}";
        Appointment appointment = new Appointment(date, time, patient, assistant, doctor);
        Insert(key, appointment);
    }

    public void DeleteAppointment(DateTime date, TimeSpan time)
    {
        string key = $"{date.ToShortDateString()} {time}";
        Delete(key);
    }

    public void ChangeAppointment(DateTime oldDate, TimeSpan oldTime, DateTime newDate, TimeSpan newTime)
    {
        string oldKey = $"{oldDate.ToShortDateString()} {oldTime}";
        string newKey = $"{newDate.ToShortDateString()} {newTime}";

        if (AppointmentList.ContainsKey(oldKey))
        {
            Appointment appointment = AppointmentList[oldKey];
            AppointmentList.Remove(oldKey);
            appointment.Date = newDate;
            appointment.Time = newTime;
            AppointmentList.Add(newKey, appointment);
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // Example usage of classes
        Doctor doctor = new Doctor { Name = "Dr. Smith", Username = "drsmith", Password = "1234" };
        Assistant assistant = new Assistant { Name = "John", Username = "john", Password = "4321" };
        Patient patient = new Patient { PatientName = "Jane Doe", Age = 30, Address = "123 Street" };

        DB db = new DB();

        // Adding an appointment
        DateTime appointmentDate = DateTime.Now;
        TimeSpan appointmentTime = new TimeSpan(10, 0, 0); // 10:00 AM
        db.AddAppointment(appointmentDate, appointmentTime, patient, doctor, assistant);

        // Displaying appointments
        foreach (var appointment in db.Select())
        {
            Console.WriteLine($"Appointment on {appointment.Date} at {appointment.Time} for {appointment.Patient.PatientName}");
        }

        // Modifying an appointment
        DateTime newAppointmentDate = appointmentDate.AddDays(1);
        db.ChangeAppointment(appointmentDate, appointmentTime, newAppointmentDate, appointmentTime);
    }
}
