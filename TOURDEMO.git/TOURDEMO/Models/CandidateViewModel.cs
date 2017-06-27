using CRM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TOURDEMO.Models
{
    public class CandidateViewModel
    {
        public CandidateViewModel()
        {
            listCandidates = new List<CandidateDetailViewModel>();
            listTags = new List<TagsViewModel>();
            candidateDetail = new tbl_Candidate();
        }
        public IList<CandidateDetailViewModel> listCandidates { get; set; }
        public IList<TagsViewModel> listTags { get; set; }
        public tbl_Candidate candidateDetail { get; set; }
    }
    public class CandidateDetailViewModel
    {
        public bool IsDelete { get; set; }
        public string Code { get; set; }
        public int Id { get; set; }
        public string Fullname { get; set; }
        public DateTime Birthday { get; set; }
        public bool Gender { get; set; }
        public DateTime ApplyDate { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int Birthplace { get; set; }
        public string IdentityCard { get; set; }
        public DateTime CreatedDateIdentity { get; set; }
        public int IdentityTagId { get; set; }
        public string ImageLink { get; set; }
        public string InformationTechnology { get; set; }
        public int HeadQuarterName { get; set; }
    }
}