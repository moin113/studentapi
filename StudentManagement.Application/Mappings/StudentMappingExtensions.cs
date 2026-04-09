using StudentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Application.Mappings
{
    public static class StudentMappingExtensions
    {
        public static Student ToDomain(this object tblObj)
        {
            dynamic tbl = tblObj;
            return new Student()
            {
                Id = tbl.Id,
                Name = tbl.Name ?? string.Empty,
                Email = tbl.Email ?? string.Empty,
                Age = tbl.Age,
                Course = tbl.Course ?? string.Empty,
                CreatedDate = tbl.CreatedDate
            };
        }

        public static object ToEntity(this Student domain)
        {
            // We use an anonymous object that acts as the entity for insertion
            // Since we can't reference TblStudent explicitly without circular dependency
            return new
            {
                Id = domain.Id,
                Name = domain.Name,
                Email = domain.Email,
                Age = domain.Age,
                Course = domain.Course,
                CreatedDate = domain.CreatedDate
            };
        }
    }
}
