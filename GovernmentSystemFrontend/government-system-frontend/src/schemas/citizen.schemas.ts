import { z } from "zod";

const beAtLeast18YearsOld = (dobString: string) => {
  if (!dobString) return false;
  const [year, month, day] = dobString.split("-").map(Number);
  if (!year || !month || !day) return false;

  const dob = new Date(year, month - 1, day);
  if (
    dob.getFullYear() !== year ||
    dob.getMonth() !== month - 1 ||
    dob.getDate() !== day
  ) {
    return false;
  }

  const today = new Date();
  let age = today.getFullYear() - dob.getFullYear();
  const monthDiff = today.getMonth() - dob.getMonth();

  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < dob.getDate())) {
    age--;
  }
  return age >= 18;
};

const citizenFormSchema = z.object({
  nationalId: z
    .string()
    .trim()
    .min(1, "National ID is required")
    .max(100, "Maximum 100 characters"),
  firstName: z
    .string()
    .trim()
    .min(1, "First name is required")
    .max(100, "Maximum 100 characters"),
  lastName: z
    .string()
    .trim()
    .min(1, "Last name is required")
    .max(100, "Maximum 100 characters"),
  dateOfBirth: z
    .string()
    .min(1, "Date of birth is required")
    .refine(beAtLeast18YearsOld, {
      message: "The citizen must be at least 18 years old.",
    }),
  governorate: z
    .number()
    .int({ message: "Governorate must be an number from 1 to 5" })
    .min(1)
    .max(5),
  gender: z
    .string()
    .min(1, "Gender is required")
    .refine((val) => val.toUpperCase() === "M" || val.toUpperCase() === "F", {
      message: "Gender must be either 'M' or 'F'",
    }),
});

export const createCitizenFormSchema = citizenFormSchema.omit({
  nationalId: true,
});

export const updateCitizenFormSchema = citizenFormSchema;

export const fetchCitizenFormSchema = citizenFormSchema.omit({
  dateOfBirth: true,
  governorate: true,
  gender: true,
  firstName: true,
  lastName: true,
});
