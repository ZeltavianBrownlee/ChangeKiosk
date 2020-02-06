using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeBot1
{
    class Program
    {
        #region GLOBAL VARIABLES
        struct Bank 
        {
            public int stored; // how many of each is stored in the bank (upon start up)
        }
        static Bank[] money;
        static decimal cashBackAmount = 0.0m;
        #endregion

        static void Main(string[] args)
        {
            #region VARIABLES
            //Arrays cash
            string[] moneyNames = { "One Hundred", "Fifty", "Twenty", "Ten", "Five", "Dollar", "Half Dollar", "Quarter", "Dime", "Nickel", "Penny" };
            decimal[] moneyValue = { 100.00m, 50.00m, 20.00m, 10.00m, 5.00m, 1.00m, .50m, .25m, .10m, .05m, .01m };
            int[] drawerBillCount = { 1, 2, 4, 5, 8, 21, 5, 20, 28, 20, 20 };
            //int[] drawerBillCount1 = { 1, 1, 0, 1, 4, 1, 1, 1, 1, 1, 1 }; //test array
            money = new Bank[moneyNames.Length];//create an instance of money

            //string[] cardNames = { "Visa", "MasterCard", "American Express", "Discover" };
           

            //declare and intialize variables 
            decimal itemPrice, purchasePrice = 0.00m;
            decimal remaining = 0.00m;
            decimal payment, totalPayment = 0.00m;
            int itemNum = 1;
            int paymentNum = 1;
            string input, formattedInput = "";
            decimal changeDue = 0.00m;
            int index = 0;
            bool transactionOn = true;
            string transactionResponse = "";

            string useCard = "";
            bool payCard = false;
            long cardNumber = 0;
            string convertedCardNumber = "";
            bool validCardNum = false;
            long cardFirstDigit = 0;
            string cardVendor = "";
            bool checkValidCard = false;
            string cashBackCheck = "";
            //decimal cashBackAmount = 0.0m;
            decimal cashBackPurchasePrice =0.0m;
            decimal valBankRequest = 0.0m;
            decimal remainingCashBackPrice = 0.0m;
            

            #endregion

            #region  BOOT UP DATA FOR CASH DRAWER CHECK
            //SETUP BOOT UP CASH
            while (index < 11)
            //store preloaded information into kiosk
            {
                money[index].stored = drawerBillCount[index];//global money drawer array set to local money drawer
                index++;
            }
            #endregion

            #region SCANNING ITEMS
            while (transactionOn)//loop to allow customers to continue making transactions
            {
                //Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("You may begin scanning your items!\n");

                // loop to get price of items being purchased
                do
                {
                    Console.Write("Item {0}: ", itemNum);

                    //break if no input value
                    input = Console.ReadLine();
                    if (input == "")
                    {
                        break;
                    }//end if

                    //format the discount price to have two decimal places
                    formattedInput = string.Format("{0:0.00}", input);

                    //convert input price to double for calculations
                    itemPrice = Convert.ToDecimal(input);

                    Console.WriteLine("Please place item in bagging area!\n");
                    itemNum += 1;
                    purchasePrice += itemPrice;
                } while (input != "");//end do while loop

                // Total price display
                Console.Write("\n\nTotal {0:c}", purchasePrice);
                #endregion

                payCard = PayWithCard(purchasePrice);
                if (payCard == true)
                {
                    CashBack(purchasePrice);
                }//end if

                if(payCard == true)
                { 
                //loop runs as long as there is no valid card number
                while (checkValidCard == false)
                {
                    //only runs if paying with a card
                    if (payCard == true)
                    {
                        Console.Write("\nPlease enter your card number:   ");
                        cardNumber = Convert.ToInt64(Console.ReadLine());

                        //convert card number to string value
                        convertedCardNumber = Convert.ToString(cardNumber);

                        //call function to get first digit
                        cardFirstDigit = CardNumberFirstDigit(cardNumber);

                        //call function to check for valid card number
                        validCardNum = IsCardNumberValid(convertedCardNumber);


                        if (validCardNum == true && cardFirstDigit == 3 || cardFirstDigit == 4 || cardFirstDigit == 5 || cardFirstDigit == 6 )
                        {

                            if (convertedCardNumber.Length >= 15 && convertedCardNumber.Length <= 16)
                            {
                                cardVendor = CardVendor(cardFirstDigit);
                               //set valid card to true
                                checkValidCard = true;
                            }
                            else
                            {
                                Console.WriteLine("Not a valid card number.");
                                checkValidCard = false;
                            }//end nested if
                        }
                        else
                        {
                            Console.WriteLine("Not a vaild card number.");
                            checkValidCard = false;
                        }//end if 

                            //checkValidCard only runs if card is valid
                            if (checkValidCard == true)
                            {
                                cashBackPurchasePrice = purchasePrice + cashBackAmount;
                                //function simulating bank sending back request
                                string[] bankRequest = MoneyRequest(convertedCardNumber, cashBackPurchasePrice);

                                if (bankRequest[1] == "declined")
                                {
                                    Console.WriteLine("Card Declined!!!!");
                                    //show remaining balance without cash back included
                                    Console.WriteLine("Remaining Balance: {0}", purchasePrice);
                                    Console.WriteLine("-----Please pay another way!-----");
                                    PayWithCard(purchasePrice);

                                    CashBack(purchasePrice);

                                    //added ways to pay then zero out remaining balance

                                }//end nested if 

                                if (bankRequest[1] != "declined")
                                {
                                    valBankRequest = Convert.ToDecimal(bankRequest[1]);
                                    //card complete pay
                                    if (valBankRequest > 0 && valBankRequest == cashBackPurchasePrice)
                                    {
                                        Console.WriteLine("Your {0} card will be charged {1}", cardVendor, valBankRequest);
                                        Console.WriteLine("------Transaction Complete!!!_____");
                                        purchasePrice = 0;
                                    }
                                    //card half pay
                                    else if (valBankRequest > 0 && valBankRequest != cashBackPurchasePrice)
                                    {
                                        Console.WriteLine("Your {0} card will be charged {1}", cardVendor, valBankRequest);

                                        remainingCashBackPrice = cashBackPurchasePrice - valBankRequest;
                                        Console.WriteLine("Your reamaing balance is {0}", remainingCashBackPrice);

                                        PayWithCard(remainingCashBackPrice);

                                        //add ways to pay then zero out remaining balance

                                    }//end nested if
                                }//end nested if
                            } //end if
                        }//end if
                    }//end if 
                }//end while loop
                if(payCard == false)
                {

                    //loop to accept payment amounts 
                    while (totalPayment <= purchasePrice)
                    {
                        Console.Write("\n\nPayment {0}  ", paymentNum);
                        payment = Convert.ToDecimal(Console.ReadLine());


                        //check payment for valid payment amount
                        bool paymentCheck = CashPaymentCheck(payment, moneyValue, drawerBillCount); //set bool equal to PaymentCheck function

                        if (paymentCheck == false)
                        { 
                        
                            Console.WriteLine("Please enter a valid payment amount.");
                            totalPayment = totalPayment - payment;
                            paymentNum -= 1;
                        }//end if

                        totalPayment += payment;
                        remaining = purchasePrice - totalPayment; //subtract payment from purchase price and set it to remaining
                        paymentNum += 1;

                        if (totalPayment > purchasePrice)
                        {
                            break;//exit loop if total payment is higher than sum of items
                        }//end if 

                        //Remaining change to be payed
                        Console.Write("Remaining  {0:c}", remaining);
                    }//end while loop

                    remaining = Math.Round(remaining, 2);
                    //Absoulte value of change (gets rid of negative sign)
                    changeDue = Math.Abs(remaining);

                    //Change due
                    Console.Write("\n\nChange     {0:c}", changeDue);
                    Console.Write("\n_ _ _ _ \n\n");

                    //call enoughChange function
                    bool enoughChange = EnoughChange(changeDue, moneyValue);

                    if (enoughChange == false)//there is enough money
                    {
                        //display message if enoughChange equals false (there is not enough money)
                        Console.WriteLine("Kiosk does not have enough money to dispense change! Please enter another form of payment.");
                    }//end if

                    ChangeDispense(changeDue, enoughChange, moneyValue, drawerBillCount);//dispense change
                }//end if

                paymentNum = 1;//reset paymentNum
                itemNum = 1;//reset itemNum
                cashBackAmount = 0;//reset cashback
                checkValidCard = false;//reset checkValidCard




                Console.Write("\nDo you want to do another transaction? Please enter y or n:  ");
                transactionResponse = Console.ReadLine();

                if (transactionResponse == "No" || transactionResponse == "N" || transactionResponse == "n" || transactionResponse == "no")
                {
                    transactionOn = false;//turns off transaction loop
                }//end if
                Console.WriteLine("\n");
            }//end while loop
            Console.ReadKey();
        }//end main function


        #region DISPENSE CHANGE
        public static void ChangeDispense(decimal change_needed, bool enoughChange, decimal[]moneyAmount, int[]drawerAmount)
        {
            int count = 0;
            //loop through to display dispensed money
            while (change_needed > 0 && enoughChange == true)
            { 
                if (change_needed >= moneyAmount[count] && drawerAmount[count] >0)
                {
                    change_needed -= moneyAmount[count];
                    change_needed = Math.Round(change_needed, 2);
                    drawerAmount[count] -= 1;
                        
                   Console.WriteLine($"{moneyAmount[count]:c} dispensed");//display the dispensed change

                    if (change_needed == 0)
                    {
                        Console.WriteLine("\n-----Transaction Complete-----");

                    }//end nested if
                }
                else if (drawerAmount[count] <0)
                {
                    count += 2;
                }

                else
                {
                    count += 1;

                }//end if              
            }//end while loop
        }//end function
        #endregion

        #region ENOUGH CHANGE
        public static bool EnoughChange(decimal change_due, decimal[] moneyValue)
        {
            int i = 0;
            //loop through to kiosk for proper change
            while (change_due > 0  )
            {
                if(money[i].stored== 0)
                {
                    i +=1;
                }
                else if (change_due >= moneyValue[i] && money[i].stored >0)
                {
                    change_due -= (moneyValue[i]);
                    change_due= Math.Round(change_due, 2);
                    //subtracts array element to check for same bill/coin in case there is more than 1
                    money[i].stored -= 1;
                    //increments to next array element if stored is equal to zero
                    if (money[i].stored == 0 && i != 10 )
                    {
                        i += 1;

                        if(i > 10 )
                        {
                            return false;
                        }//end nested if    


                    }//end nested if

                    //returns false if money makes it to the end of money drawer without zeroing out
                    if(money[10].stored == 0 && i == 10)
                    {
                        return false;
                    }//end nested if

                    //returns true if change zeros out
                    if( money[i].stored >= 0 && change_due == 0)
                    {
                        return true;// make bool true if no change remains and bill/coin count is zero or more
                    }//end nested if
                }
                else
                {
                    i += 1;
                    if(i>10)
                    {
                        return false;
                    }//end nested if
                }//end if

            }//end while loop
            return false;
        }//end function  

        #endregion

        #region CHECK RIGHT PAYMENT AMOUNT
        public static bool CashPaymentCheck (decimal payments, decimal[] moneyValue, int[] moneyStored)
        {
            for(int x = 0; x<moneyValue.Length;x++)
            {
                if(payments == moneyValue[x])
                {
                    moneyStored[x] += 1;//increase value of money drawer
                    return true;
                }//end if

                if(payments != moneyValue[x] && x == 10)
                {
                     return false;
                }//end if
            }//end for loop
            return false;
        }//end function
        #endregion

        #region CARD FIRST DIGIT
        public static int CardNumberFirstDigit(long cardNum)
        {
            int cardFirstDigit = 0;
            //Display card vendor
            cardFirstDigit = (int)(cardNum.ToString()[0]-48);
            return cardFirstDigit;
        }//end function
        #endregion

        #region CARD VENDOR
        public static string CardVendor(long cardFirstDig)
        {
            if (cardFirstDig== 3)
            {
                return "American Express";
            }

            else if (cardFirstDig == 4)
            {
                return "Visa";
            }
            else if (cardFirstDig == 5)
            {
                return "MasterCard";
            }
            else if (cardFirstDig== 6)
            {
                return "Discover";
            }
            else
            {
                return "a";
            }//end if 
        }//end function
        #endregion

        #region PAY WITH CARD
        static bool PayWithCard(decimal purchasePrice)
        {
            string useCard = "";
            do
            {
                //loop to check if paying with a card
                Console.Write("\n\nWould you like to pay with a credit card? Please enter y or n.   ");
                useCard = Console.ReadLine();

                //set useCard to true if customer wants to use card
                if (useCard == "Y" || useCard == "y" || useCard == "yes" || useCard == "Yes")
                {
                    return true;
                }//end if
                 //set useCard to true if customer wants to use card
                else if (useCard == "N" || useCard == "n" || useCard == "no" || useCard == "No")
                {
                    return false;
                }//end if
                else
                {
                    Console.WriteLine("Please enter a valid response!!!!");
                }//end if

            } while (useCard != "Y" && useCard != "y" && useCard != "yes" && useCard != "Yes" && useCard != "N" && useCard != "n" && useCard != "no" && useCard != "No");//end while loop
        
            return false;
        }//end function
        #endregion

        #region CASH BACK
        static bool CashBack(decimal purchasePrice)
        {
            string cashBackCheck = "";
            decimal cashBackPurchasePrice = 0.0m;
           

            //loop checking for cashback
            do
            {
                Console.Write("Would you like cash back?  ");
                cashBackCheck = Console.ReadLine();

                if (cashBackCheck == "Y" || cashBackCheck == "y" || cashBackCheck == "yes" || cashBackCheck == "Yes")
                {
                    Console.Write("Enter cashback amount:  ");
                    cashBackAmount = Convert.ToDecimal(Console.ReadLine());
                    cashBackPurchasePrice = purchasePrice + cashBackAmount;
                    //Console.WriteLine($"Your card will be charged: {cashBackPurchasePrice}");
                    return true;
                }
                else if ((cashBackCheck == "N" || cashBackCheck == "n" || cashBackCheck == "no" || cashBackCheck == "No"))
                {
                    cashBackPurchasePrice = purchasePrice + 0;
                    return false;
                }
                else
                {
                    Console.WriteLine("Please enter a valid Response!!!!!!");
                }//end if

            } while (cashBackCheck != "Y" && cashBackCheck != "y" && cashBackCheck != "yes" && cashBackCheck != "Yes" && cashBackCheck != "N" && cashBackCheck != "n" && cashBackCheck != "no" && cashBackCheck != "No");//end while loop
            return false;
        }//end function
    #endregion

        #region CHECK VALID CARD NUMBER
        public static bool IsCardNumberValid(string cardNumber)
        {
            int i,checkSum = 0;

            // Compute checksum of every other digit starting from right-most digit
            for (i = cardNumber.Length - 1; i >= 0; i -= 2)
                checkSum += (cardNumber[i] - '0');

            /*Now take digits not included in first checksum, multiple by two,
            and compute checksum of resulting digits*/
            for (i = cardNumber.Length - 2; i >= 0; i -= 2)
            {
                int val = ((cardNumber[i] - '0') * 2);
                while (val > 0)
                {
                    checkSum += (val % 10);
                    val /= 10;
                }//end while loop
            }//end for loop

            // Number is valid if sum of both checksums MOD 10 equals 0
            return ((checkSum % 10) == 0);
        }//end function
        #endregion

        #region BANK MONEY REQUEST
        static string[]MoneyRequest(string account_number, decimal amount)
        {
            Random rnd = new Random();
            //50% CHANCE TRANSACTION PASSES OR FAILS
            bool pass = rnd.Next(100) < 50;
            //50% CHANCE THAT A FAILED TRANSACTION IS DECLINED
            bool declined = rnd.Next(100) < 50;

            if (pass)
            {
                return new string[] { account_number, amount.ToString() };

            }
            else
            {
                if (!declined)
                {
                    return new string[] { account_number, (amount / rnd.Next(2, 6)).ToString() };
                }
                else
                {
                    return new string[] { account_number, "declined" };
                }//end nested if
            }//end if         
        }//end function
        #endregion

        //#region BANK DECLINED FUCNTION

        //static void BankDeclinedCard()
        //{
        //    string newPaymentMethod= "";

        //    Console.WriteLine("!-- Card Declined --!\n");
           
        //    do
        //    {
        //        //loop to check if paying with a card
        //        Console.WriteLine("--- Pay a different way?");
        //        newPaymentMethod= Console.ReadLine();
        //        string cardPay = "";

        //        //set useCard to true if customer wants to use card
        //        if (newPaymentMethod == "Y" || newPaymentMethod == "y" || newPaymentMethod == "yes" ||  newPaymentMethod == "Yes")
        //        {
                    
                    
        //        }//end if
        //         //set useCard to true if customer wants to use card
        //        else if (newPaymentMethod == "N" || newPaymentMethod == "n" || newPaymentMethod== "no" || newPaymentMethod== "No")
        //        {
                    
        //        }//end if
        //        else
        //        {
        //            Console.WriteLine("Please enter a valid response!!!!");
        //        }

        //    } while (newPaymentMethod != "Y" && newPaymentMethod != "y" && newPaymentMethod != "yes" && newPaymentMethod != "Yes" && newPaymentMethod != "N" && newPaymentMethod != "n" && newPaymentMethod != "no" && newPaymentMethod != "No");//end while loop   
        //}//end function

        //#endregion
    }//end program class
}//end namespace

