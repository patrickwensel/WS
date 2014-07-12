using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Objects;
using System.Linq;
using NLog;
using WS.Framework.Objects.Employees;
using WS.Framework.ServicesInterface;
using WS.Framework.WSJDEData;

namespace WS.Framework.ServicesInterfaceImplementation
{
    public class EmployeeService : IEmployeeService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public EmployeeSearch GetEmployeeSearch(EmployeeSearch employeeSearch)
        {
            try
            {
                using (WSJDE context = new WSJDE())
                {
                    IQueryable<Employee> query = context.Employees;

                    if (!String.IsNullOrEmpty(employeeSearch.Name))
                    {
                        query =
                            query.Where
                                (e =>
                                 (e.FirstName.Contains(employeeSearch.Name) || e.LastName.Contains(employeeSearch.Name)) && e.EmployeeStatusID == employeeSearch.EmployeeStatusID);
                    }
                    else
                    {
                        query =
                            query.Where
                                (e => e.EmployeeStatusID == employeeSearch.EmployeeStatusID);
                    }

                    query = AddSortingToGetEmployeeSearch(query, employeeSearch.Sorting);

                    employeeSearch.TotalRecords = query.Count();

                    query = query.Skip(employeeSearch.StartIndex).Take(employeeSearch.Count);

                    employeeSearch.Employees = query.ToList();

                    return employeeSearch;

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return null;
        }

        private IQueryable<Employee> AddSortingToGetEmployeeSearch(IQueryable<Employee> query, string sorting)
        {
            switch (sorting)
            {
                case "FirstName ASC":
                    query = query.OrderBy(o => o.FirstName);
                    break;

                case "FirstName DESC":
                    query = query.OrderByDescending(o => o.FirstName);
                    break;

                case "LastName ASC":
                    query = query.OrderBy(o => o.LastName);
                    break;

                case "LastName DESC":
                    query = query.OrderByDescending(o => o.FirstName);
                    break;

                case "Title ASC":
                    query = query.OrderBy(o => o.Title);
                    break;

                case "Title DESC":
                    query = query.OrderByDescending(o => o.Title);
                    break;

                default:
                    query = query.OrderBy(o => o.LastName);
                    break;
            }
            return query;
        }

        public Employee GetEmployeeByEmployeeNumber(string employeeNumber)
        {
            Employee employee = new Employee();

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    employee = (from e in context.Employees
                                where e.EmployeeNumber == employeeNumber
                                select e).FirstOrDefault();

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return employee;
        }

        public string GetEmployeeIDByADUserName(string adUserName)
        {
            string employeeID = "";

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    employeeID = (from e in context.Employees
                                  where e.ADUserName == adUserName
                                select e.ID).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return employeeID;

        }

        public int GetEmployeeBranchByADUserName(string adUserName)
        {
            int branch = 0;

            try
            {
                using (WSJDE context = new WSJDE())
                {
                    int employeeBranch = Convert.ToInt32((from e in context.Employees
                                                          where e.ADUserName == DbFunctions.AsNonUnicode(adUserName)
                                                          select e.ReportLocationID).FirstOrDefault());

                    string branchString = employeeBranch.ToString().PadLeft(12);

                    var businessUnit = (from bu in context.BusinessUnits
                                                 where bu.ID == DbFunctions.AsNonUnicode(branchString)
                                                 select bu);

                    if (businessUnit.Any())
                    {
                        branch = employeeBranch;
                    }

                }
            }
            catch (Exception e)
            {
                logger.ErrorException("Exception", e);
            }

            return branch;
        }

       
    }
}
