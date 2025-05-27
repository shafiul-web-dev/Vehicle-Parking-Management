﻿using System;
using System.ComponentModel.DataAnnotations;

namespace ParkingManagementAPI.Validations
{
	public class FutureDateAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			if (value is DateTime date)
			{
				return date > DateTime.Now;
			}
			return false;
		}
	}
}