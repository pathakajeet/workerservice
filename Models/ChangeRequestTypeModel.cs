using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerService.Models
{
    public class ChangeRequestTypeModel
    {

        public string f_type { get; set; }
        public string projectCode { get; set; }
        public string OBJ_CODE { get; set; }
        public string Title { get; set; }
        public string BDG_CHANGE_TYPE { get; set; }
        public DateTime? OBJ_DATE { get; set; }
        public DateTime? OBJ_DUE_DATE { get; set; }
        public decimal? OBJ_CREATOR { get; set; }
        public decimal? WF_ID { get; set; }
        public long? WF_STEP { get; set; }
        public int FORM_VERSION { get; set; }
        public bool IS_DELETE { get; set; }
        public bool IS_PR { get; set; }
    }
}
