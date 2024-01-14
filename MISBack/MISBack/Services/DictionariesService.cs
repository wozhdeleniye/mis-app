using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services
{
    public class DictionariesService : IDictionariesService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public DictionariesService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<SpecialtiesPagedListModel> GetSpecialities(string name, int page, int size)
        {
            var specialitiesEntity = await _context
                .Specialities
                .Where(x=> x.name.Contains(name)).ToListAsync();

            int start = (page - 1) * size;
            int end = Math.Min(size, specialitiesEntity.Count - start);
            double count = Double.Ceiling((double)specialitiesEntity.Count / size);
            if (page > count)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                    "Out of page list");
                throw ex;
            }
            SpecialtiesPagedListModel specialitiesList = new SpecialtiesPagedListModel();
            specialitiesList.pagination = new PageInfoModel
            {
                size = size,
                count = (int)count,
                current = page

            };
            specialitiesList.specialities = new List<SpecialityModel>();
            specialitiesEntity.GetRange(start, end);
            foreach(Speciality spec in specialitiesEntity)
            {
                specialitiesList.specialities.Add(_mapper.Map<SpecialityModel>(spec));
            }
            return specialitiesList;
        }

        public async Task<Icd10SearchModel> SearchIcd10(string request, int page, int size)
        {
            var Icd10sEntity = await _context
                .Icd10s
                .Where(x=> x.name.Contains(request) || x.code.Contains(request)).ToListAsync();

            int start = (page - 1) * size;
            int end = Math.Min(size, Icd10sEntity.Count - start);
            double count = Double.Ceiling((double)Icd10sEntity.Count / size);
            if (page > count)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                    "Out of page list");
                throw ex;
            }

            Icd10SearchModel icd10sList = new Icd10SearchModel();
            icd10sList.pagination = new PageInfoModel
            {
                size = size,
                count = (int)count,
                current = page

            };
            icd10sList.records = new List<Icd10RecordModel>();

            Icd10sEntity.GetRange(start, end);
            foreach (Icd10 icd10 in Icd10sEntity)
            {
                icd10sList.records.Add(_mapper.Map<Icd10RecordModel>(icd10));
            }

            return icd10sList;
        }

        public async Task<List<Icd10RecordModel>> GetRootIcd()
        {
            var Icd10sEntity = await _context
                .Icd10s
                .Where(x => x.parentId == null).ToListAsync();
            List<Icd10RecordModel>  icdList = new List<Icd10RecordModel>();
            foreach(Icd10 icd10 in Icd10sEntity)
            {
                icdList.Add(_mapper.Map<Icd10RecordModel>(icd10));
            }
            return icdList;
        }
    }
}
