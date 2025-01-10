using System;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Models
{
	public class Compensation
	{
		[Key]
		public string EmployeeId { get; set; }
		public decimal Salary { get; set; }
		public DateTime EffectiveDate { get; set; }
	}
}
