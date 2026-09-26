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

export const citizenFormSchema = z.object({
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
  governorate: z.string().min(1, "Please select a governorate"),
  gender: z
    .string()
    .min(1, "Gender is required")
    .refine((val) => val.toUpperCase() === "M" || val.toUpperCase() === "F", {
      message: "Gender must be either 'M' or 'F'",
    }),
});

export type CitizenFormValues = z.infer<typeof citizenFormSchema>;
