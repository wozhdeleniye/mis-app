using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;
using System.Security.Principal;

namespace MISBack.Services
{
    public class ReportsService : IReportsService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ReportsService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<IcdRootsRepotModel> GetInspections(DateTime start, DateTime end, List<int> icdRoots)
        {
            IcdRootsRepotModel  report = new IcdRootsRepotModel();
            report.records = new List<IcdRootsReportRecordModel>();
            report.summaryByRoot = new Dictionary<string, int>();
            if(icdRoots.Count == 0)
            {
                var icdRootsEntity = await _context
                    .Icd10s
                    .Where(x => x.parentId == null).ToListAsync();
                foreach(var icdRoot in icdRootsEntity)
                {
                    icdRoots.Add(icdRoot.id);
                }
            }
            var patsEntity = await _context
                .Patients.ToListAsync();
            Dictionary<String, int> patientsDic = new Dictionary<String, int>();

            foreach (var icd in icdRoots)
            {
                var icdEntity = await _context
                    .Icd10s
                    .FirstOrDefaultAsync(x => x.id == icd);
                patientsDic.Add(icdEntity.code, 0);
            }

            foreach (var p in patsEntity)
            {
                var inspecsEntity = await _context
                    .Inspections
                    .Where(x => x.patientId == p.id && x.date >= start && x.date <= end).ToListAsync();
                Dictionary<String, int> patientDic = new Dictionary<String, int>();
                foreach(var icd in icdRoots)
                {
                    var icdEntity = await _context
                        .Icd10s
                        .FirstOrDefaultAsync(x => x.id == icd);
                    patientDic.Add(icdEntity.code, 0);
                }
                foreach (var inspec in inspecsEntity) 
                {
                    var diagEntity = await _context
                        .Diagnoses
                        .FirstOrDefaultAsync(x => x.InspectionId == inspec.id && x.type == DiagnosisType.Main);
                    var icdEntity = await _context
                        .Icd10s
                        .FirstOrDefaultAsync(x => x.id == diagEntity.icd10Id);
                    while (icdEntity.parentId != null)
                    {
                        icdEntity = await _context
                            .Icd10s
                            .FirstOrDefaultAsync(x => x.id == icdEntity.parentId);
                    }
                    if (patientDic.ContainsKey(icdEntity.code))
                    {
                        patientDic[icdEntity.code] += 1;
                        patientsDic[icdEntity.code] += 1;
                    }

                }
                report.records.Add(new IcdRootsReportRecordModel
                {
                    patientName = p.name,
                    patientBirthdate = p.birthday,
                    gender = p.gender,
                    visitsByRoot = patientDic
                });
            }
            report.summaryByRoot = patientsDic;
            report.filters = new IcdRootsRepotFiltersModel
            {
                start = start,
                end = end,
                icdRoots = icdRoots
            };
            return report;
        }
    }
}
