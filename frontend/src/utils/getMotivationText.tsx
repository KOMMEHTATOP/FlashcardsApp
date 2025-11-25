import { levelMotivationVariants, motivationTexts } from "@/shared/data";

const getMotivationText = (score: number): string => {
  if (score === 100) return motivationTexts["100"];
  if (score <= 20) return motivationTexts["0-20"];
  if (score <= 40) return motivationTexts["21-40"];
  if (score <= 60) return motivationTexts["41-60"];
  if (score <= 80) return motivationTexts["61-80"];
  if (score <= 94) return motivationTexts["81-94"];
  if (score >= 95) return motivationTexts["95-100"];
  return "✨ Продолжай в том же духе!";
};

export function getLevelMotivationText(
  currentXP: number,
  xpToNext: number
): string {
  const progress = Math.floor((currentXP / xpToNext) * 100);

  let key: string;

  if (progress >= 100) key = "100";
  if (progress >= 100) key = "100";
  else if (progress <= 10) key = "0-10";
  else if (progress <= 30) key = "11-30";
  else if (progress <= 60) key = "31-60";
  else if (progress <= 90) key = "61-90";
  else key = "91-99";

  const option = levelMotivationVariants[key];
  const randomIndex = Math.floor(Math.random() * option.length);

  if (option[randomIndex]) return option[randomIndex];
  return "📘 Продолжай в том же духе!";
}

export default getMotivationText;
