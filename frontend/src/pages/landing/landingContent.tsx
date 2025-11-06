import {
  Brain,
  BookOpen,
  Star,
  TrendingUp,
  Trophy,
  Zap,
  Sparkles,
  Target,
  Flame,
  Award,
  Users,
  Clock,
} from "lucide-react";
import type { ComponentType, SVGProps } from "react";

type IconType = ComponentType<SVGProps<SVGSVGElement>>;

export type Feature = {
  icon: IconType;
  title: string;
  description: string;
  gradient: string;
};

export type Stat = {
  icon: IconType;
  value: string;
  valuePrefix?: string;
  label: string;
  gradient: string;
};

export type Step = {
  number: string;
  title: string;
  description: string;
  gradient: string;
};

const LandingContentService = class {
  static getFeatures(): Feature[] {
    return [
      {
        icon: Brain,
        title: "Карточки для запоминания",
        description:
          "Создавайте карточки с вопросом и ответом для изучения любой информации",
        gradient: "from-blue-400 to-cyan-500",
      },
      {
        icon: BookOpen,
        title: "Группы карточек",
        description:
          "Организуйте карточки по темам: английский, программирование, история",
        gradient: "from-purple-400 to-pink-500",
      },
      {
        icon: Star,
        title: "Система оценок",
        description:
          "Оценивайте свои знания от 1 до 5 звёзд после изучения каждой карточки",
        gradient: "from-yellow-400 to-orange-500",
      },
      {
        icon: TrendingUp,
        title: "Детальная статистика",
        description:
          "Отслеживайте прогресс, время обучения и количество изученных карточек",
        gradient: "from-green-400 to-emerald-500",
      },
      {
        icon: Trophy,
        title: "Достижения",
        description:
          "Получайте награды за ударный режим, количество карточек, уровни",
        gradient: "from-amber-400 to-orange-500",
      },
      {
        icon: Zap,
        title: "Уровни и XP",
        description: "Прокачивайте уровень, зарабатывая опыт за изучение",
        gradient: "from-indigo-400 to-purple-500",
      },
    ];
  }

  static getGamificationFeatures(): Feature[] {
    return [
      {
        icon: Sparkles,
        title: "Система опыта (XP)",
        description:
          "Получайте XP за каждую карточку. Чем выше оценка, тем больше XP",
        gradient: "from-cyan-400 to-blue-500",
      },
      {
        icon: Target,
        title: "Уровни",
        description:
          "Повышайте свой уровень, набирая опыт. Каждый уровень открывает новые возможности",
        gradient: "from-violet-400 to-purple-500",
      },
      {
        icon: Flame,
        title: "Ударный режим",
        description:
          "Занимайтесь каждый день подряд и получайте бонусный XP. Побейте свой рекорд!",
        gradient: "from-red-400 to-pink-500",
      },
      {
        icon: Award,
        title: "Достижения",
        description:
          "Разблокируйте достижения: Обычные, Редкие, Эпические, Легендарные",
        gradient: "from-lime-400 to-green-500",
      },
    ];
  }

  static getSteps(): Step[] {
    return [
      {
        number: "01",
        title: "Зарегистрируйтесь",
        description: "Создайте бесплатный аккаунт за 30 секунд",
        gradient: "from-blue-400 to-cyan-500",
      },
      {
        number: "02",
        title: "Создайте группу",
        description: 'Добавьте первую группу карточек (например, "Английский")',
        gradient: "from-purple-400 to-pink-500",
      },
      {
        number: "03",
        title: "Добавьте карточки",
        description: "Заполните группу карточками для изучения",
        gradient: "from-yellow-400 to-orange-500",
      },
      {
        number: "04",
        title: "Начните изучение",
        description: "Запустите сессию и прокачивайте свой уровень!",
        gradient: "from-green-400 to-emerald-500",
      },
    ];
  }

  static getStats(): Stat[] {
    return [
      {
        icon: Users,
        value: "100",
        valuePrefix: "+",
        label: "Активных пользователей",
        gradient: "from-blue-400 to-cyan-500",
      },
      {
        icon: BookOpen,
        value: "5000",
        valuePrefix: "+",
        label: "Карточек создано",
        gradient: "from-purple-400 to-pink-500",
      },
      {
        icon: Clock,
        value: "1000",
        valuePrefix: "+",
        label: "Часов обучения",
        gradient: "from-yellow-400 to-orange-500",
      },
      {
        icon: Flame,
        value: "7",
        valuePrefix: "дней",
        label: "Средний ударный режим",
        gradient: "from-red-400 to-pink-500",
      },
    ];
  }

  static getBenefits(): string[] {
    return [
      "Занимайтесь регулярно и сохраняйте ударный режим",
      "Честно оценивайте знания для лучших результатов",
      "Создавайте компактные карточки",
      "Следите за статистикой прогресса",
      "Гонитесь за достижениями",
      "Не прерывайте ударный режим - даже 10 минут в день делают разницу",
    ];
  }
};

export { LandingContentService };
