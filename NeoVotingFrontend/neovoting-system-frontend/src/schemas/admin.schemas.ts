import { z } from "zod";

export const electionCreateSchema = z.object({
  name: z
    .string()
    .min(3, "Minimum 3 characters")
    .max(100, "Maximum 100 characters"),
  nominationStartDate: z.string().min(1, "Nomination Start Date is required"),
  nominationEndDate: z.string().min(1, "Nomination End Date is required"),
  votingStartDate: z.string().min(1, "Voting Start Date is required"),
  votingEndDate: z.string().min(1, "Voting End Date is required"),
});

export const pollCreateSchema = z.object({
  name: z
    .string()
    .min(3, "Minimum 3 characters")
    .max(100, "Maximum 100 characters"),
  question: z
    .string()
    .min(3, "Minimum 3 characters")
    .max(4000, "Maximum 4000 characters"),
  startDate: z.string().min(1, "Start Date is required"),
  endDate: z.string().min(1, "End Date is required"),
  answers: z
    .array(z.string().min(1, "Answer cannot be empty").max(4000))
    .min(2, "At least two answers are required"),
});
