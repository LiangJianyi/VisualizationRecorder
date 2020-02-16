#pragma once
#include "../../SundryUtilty/Cpp code files/JanyeeDateTime/DateTime.h"

struct MainPageHelper
{
	static Janyee::DateTime RandomDateTime(Janyee::DateTime const& lowerLimitDate, Janyee::DateTime const& upperLimitDate) {
		auto const& lowerLimitYear = lowerLimitDate.GetYear();
		auto const& upperLimitYear = upperLimitDate.GetYear();
		auto const& lowerLimitMonth = lowerLimitDate.GetMonth();
		auto const& upperLimitMonth = upperLimitDate.GetMonth();
		auto const& lowerLimitDay = lowerLimitDate.GetDayOfMonth();
		auto const& upperLimitDay = upperLimitDate.GetDayOfMonth();
		std::mt19937 gen { std::random_device{}() };
		unsigned long long year = std::uniform_int_distribution<unsigned long long>(lowerLimitYear, upperLimitYear)(gen);
		unsigned short month;
		unsigned short day;
		if (year == lowerLimitYear) {

		}
		else if (year == upperLimitYear) {

		}
		else {
			month = std::uniform_int_distribution<decltype(month)>(1, 12)(gen);
			if (month == lowerLimitMonth) {
				day = std::uniform_int_distribution<decltype(day)>(lowerLimitDay, CalculateUpperDay(year, month))(gen);
			}
			else if (month == upperLimitMonth) {
				day = std::uniform_int_distribution<decltype(day)>(1, upperLimitDay)(gen);
			}
			else {
				day = std::uniform_int_distribution<decltype(day)>(1, CalculateUpperDay(year, month))(gen);
			}
		}


		//auto month = std::uniform_int_distribution<unsigned short>(std::get<DOWN_LIMIT>(months), std::get<UPPER_LIMIT>(months))(gen);
		//auto day = std::uniform_int_distribution<unsigned short>(std::get<DOWN_LIMIT>(days), std::get<UPPER_LIMIT>(days))(gen);
		//auto hour = std::uniform_int_distribution<unsigned short>(0, 23)(gen);
		//auto min = std::uniform_int_distribution<unsigned short>(0, 59)(gen);
		//auto sec = std::uniform_int_distribution<unsigned short>(0, 59)(gen);
		//return std::make_tuple(year, month, day, hour, min, sec);
		return Janyee::DateTime(year, month, day);
	}

private:
	// 返回指定月份的天数上限
	static unsigned short CalculateUpperDay(unsigned long long const& year, unsigned short const& month) {
		std::map<int, int> monthPair;
		monthPair[1] = 31;
		monthPair[2] = Janyee::DateTime::IsLeapYear(year) ? 29 : 28;
		monthPair[3] = 31;
		monthPair[4] = 30;
		monthPair[5] = 31;
		monthPair[6] = 30;
		monthPair[7] = 31;
		monthPair[8] = 31;
		monthPair[9] = 30;
		monthPair[10] = 31;
		monthPair[11] = 30;
		monthPair[12] = 31;
		return monthPair.at(month);
	}
};

