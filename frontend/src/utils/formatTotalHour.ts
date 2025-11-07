const formatTotalHour = (time: String) => {
  if (!time || time === "0") return 0;
  const [h, m, s] = time.split(":").map(Number);
  return h + m / 60 + s / 3600;
};

export default formatTotalHour;
