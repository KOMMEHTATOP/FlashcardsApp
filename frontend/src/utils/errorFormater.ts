export const errorFormater = (error: any) => {
  const errors = error.response?.data?.errors;

  if (errors && typeof errors === "object") {
    const messages = Object.values(errors).flat().join("\n");
    return messages;
  }
};
