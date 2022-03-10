import datetime


def converter(path):
    """declare the path to csv file in to convert csv-readable format into database-readable format"""
    csvfile = open(path, newline='')

    reader = list(csvfile.readlines())

    for i in range(len(reader)):
        reader[i] = reader[i].strip().split(";")

    print(reader)

    response = [[], []]

    for i in range(len(reader)):

        for j in range(len(reader[i])):

            if j == 0:

                response[i].append(
                    datetime.datetime(int(reader[i][j][6:10]), int(reader[i][j][3:5]), int(reader[i][j][:2]),
                                      int(reader[i][j][11:13]), int(reader[i][j][14:16]), int(reader[i][j][18:])))
            elif j == 1:

                response[i].append(
                    datetime.datetime(int(reader[i][j][6:10]), int(reader[i][j][3:5]), int(reader[i][j][:2]),
                                      int(reader[i][j][11:13]), int(reader[i][j][14:16]), int(reader[i][j][18:])))

            else:
                if reader[i][j] == '':
                    response[i].append(None)
                else:
                    response[i].append(float(reader[i][j]))

    print(response)


converter("/home/kennet/PycharmProjects/PowerMonitor/server/PythonScripts/PredprofExamples/1607553426.csv")
