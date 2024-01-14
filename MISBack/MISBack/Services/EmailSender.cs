using AutoMapper;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
using Quartz;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices.JavaScript;

namespace MISBack.Services
{
    public class EmailSender : IJob
    {
        private readonly AppDbContext _context;
        public EmailSender(AppDbContext context)
        {
            _context = context;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            var patientsEntity = await _context
                .Patients
                .ToListAsync();

            foreach (var patient in patientsEntity)
            {
                var inspecsEntity = await _context
                    .Inspections
                    .ToListAsync();
                List<Inspection> list = new List<Inspection>();
                foreach(var inspec in inspecsEntity)
                {
                    var inspecEntity = await _context 
                        .Inspections
                        .FirstOrDefaultAsync(x=> x.patientId == inspec.id);

                    if(inspecEntity == null && inspecEntity.nextVisitDate != null && inspecEntity.conclusion != Data.Enums.Conclusion.Recovery && inspecEntity.conclusion != Data.Enums.Conclusion.Death)
                    {
                        if(inspecEntity.nextVisitDate >= DateTime.Now)
                        {
                            list.Add(inspecEntity);
                        }
                    }
                }
                if(list.Count > 0)
                {
                    foreach (var inspec in list)
                    {
                        var docEntity = await _context
                            .Doctors
                            .FirstOrDefaultAsync(x => x.id == inspec.docId);
                        var patEntity = await _context
                            .Patients
                            .FirstOrDefaultAsync(x => x.id == inspec.patientId);
                        string mSubject = "Пропуск посещения";
                        string message = $"Вы пропустили посещение пациента {patEntity.name}, назначенное на {inspec.nextVisitDate}";
                        try
                        {
                            MailMessage mail = new MailMessage();
                            SmtpClient smtp = new SmtpClient("smtp.gmail.com");

                            mail.From = new MailAddress("brasscout@gmail.com");
                            mail.To.Add(docEntity.email);
                            mail.Subject = mSubject;
                            mail.Body = message;

                            smtp.Port = 587;
                            smtp.Credentials = new System.Net.NetworkCredential("brasscout@gmail.com", "igromir.04");
                            smtp.EnableSsl = true;

                            smtp.Send(mail);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
        }
    }
}
