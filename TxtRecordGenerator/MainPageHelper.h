#pragma once
#include <sstream>
#include "../../SundryUtilty/Cpp code files/JanyeeDateTime/DateTime.h"

struct MainPageHelper
{
	static Janyee::DateTime RandomDateTime(Janyee::DateTime const& lowerLimitDate, Janyee::DateTime const& upperLimitDate) {
		auto lowerLimitYear = lowerLimitDate.GetYear();
		auto upperLimitYear = upperLimitDate.GetYear();
		auto lowerLimitMonth = lowerLimitDate.GetMonth();
		auto upperLimitMonth = upperLimitDate.GetMonth();
		auto lowerLimitDay = lowerLimitDate.GetDayOfMonth();
		auto upperLimitDay = upperLimitDate.GetDayOfMonth();
		std::mt19937 gen { std::random_device{}() };
		decltype(upperLimitYear) year = std::uniform_int_distribution<decltype(upperLimitYear)>(lowerLimitYear, upperLimitYear)(gen);
		decltype(upperLimitMonth) month = 0;
		decltype(upperLimitDay) day = 0;
		try {
			if (year == lowerLimitYear) {
				month = std::uniform_int_distribution<decltype(month)>(lowerLimitMonth, 12)(gen);
				if (month == lowerLimitMonth) {
					day = std::uniform_int_distribution<decltype(day)>(lowerLimitDay, CalculateUpperDay(year, month))(gen);
				}
				else {
					day = std::uniform_int_distribution<decltype(day)>(1, CalculateUpperDay(year, month))(gen);
				}
			}
			else if (year == upperLimitYear) {
				month = std::uniform_int_distribution<decltype(month)>(1, upperLimitMonth)(gen);
				if (month == upperLimitMonth) {
					day = std::uniform_int_distribution<decltype(day)>(1, upperLimitDay)(gen);
				}
				else {
					day = std::uniform_int_distribution<decltype(day)>(1, CalculateUpperDay(year, month))(gen);
				}
			}
			else {
				month = std::uniform_int_distribution<decltype(month)>(1, 12)(gen);
				day = std::uniform_int_distribution<decltype(day)>(1, CalculateUpperDay(year, month))(gen);
			}
			return Janyee::DateTime(year, month, day);
		}
		catch (const std::out_of_range& e) {
			std::stringstream s;
			s << "CalculateUpperDay raise out of range exception. Year value is "
				<< std::to_string(year)
				<< ". Month value is "
				<< std::to_string(month)
				<< ". Day value is "
				<< std::to_string(day)
				<< "\n";
			s << e.what() << "\n";
			OutputDebugStringA(s.str().c_str());
			throw std::out_of_range(s.str());
		}
	}

	static double RandomEventFrequency(double downLimit, double upperLimit) {
		unsigned seed = std::chrono::system_clock::now().time_since_epoch().count();
		std::mt19937 gen { seed };
		return std::uniform_int_distribution<decltype(downLimit)>(downLimit, upperLimit)(gen);
	}

private:
	// 返回指定月份的天数上限
	static int CalculateUpperDay(int& year, int& month) {
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

