export const isAtLeast18 = (dobString: string): boolean => {
  if (!dobString) return false;

  const parts = dobString.split("-").map(Number);
  if (parts.length !== 3) return false;

  const [year, month, day] = parts;

  // Validate all parts are real numbers
  if (!year || !month || !day) return false;

  const dob = new Date(year, month - 1, day);

  // Ensure the date is valid (e.g. rejects 2023-02-30)
  const isValidDate =
    dob.getFullYear() === year &&
    dob.getMonth() === month - 1 &&
    dob.getDate() === day;

  if (!isValidDate) return false;

  const today = new Date();

  let age = today.getFullYear() - dob.getFullYear();

  const hasHadBirthdayThisYear =
    today.getMonth() > dob.getMonth() ||
    (today.getMonth() === dob.getMonth() && today.getDate() >= dob.getDate());

  if (!hasHadBirthdayThisYear) {
    age--;
  }

  return age >= 18;
};
