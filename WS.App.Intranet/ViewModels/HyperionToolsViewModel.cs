using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WS.App.Intranet.ViewModels
{
    public class HyperionToolsViewModel
    {
        [Required]
        [DisplayName("Month")]
        public string Month { get; set; }

        [Required]
        [DisplayName("Year")]
        public string Year { get; set; }

        [Required]
        [DisplayName("Ledger Type")]
        public string LedgerType { get; set; }

        public IEnumerable<SelectListItem> Months { get; set; }
        public IEnumerable<SelectListItem> Years { get; set; }
        public IEnumerable<SelectListItem> LedgerTypes { get; set; }

        public HyperionToolsViewModel()
        {
            List<SelectListItem> months = new List<SelectListItem>()
                {
                    new SelectListItem {Value = "1", Text = "January"},
                    new SelectListItem {Value = "2", Text = "February"},
                    new SelectListItem {Value = "3", Text = "March"},
                    new SelectListItem {Value = "4", Text = "April"},
                    new SelectListItem {Value = "5", Text = "May"},
                    new SelectListItem {Value = "6", Text = "June"},
                    new SelectListItem {Value = "7", Text = "July"},
                    new SelectListItem {Value = "8", Text = "August"},
                    new SelectListItem {Value = "9", Text = "September"},
                    new SelectListItem {Value = "10", Text = "October"},
                    new SelectListItem {Value = "11", Text = "November"},
                    new SelectListItem {Value = "12", Text = "December"}
                };
            Months = months;

            List<SelectListItem> years = new List<SelectListItem>();

            for (int i = DateTime.Now.Year; i >= 2000; i--)
            {
                SelectListItem year = new SelectListItem
                    {
                        Value = i.ToString(),
                        Text = i.ToString()
                    };

                years.Add(year);
            }

            Years = years;

            List<SelectListItem> ledgerTypes = new List<SelectListItem>()
                {
                    new SelectListItem {Value = "ACTUAL", Text = "Actual"},
                    new SelectListItem {Value = "BUDGET", Text = "Budget"}
                };

            LedgerTypes = ledgerTypes;

        }

    }
}