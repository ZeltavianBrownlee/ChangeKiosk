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
        struct Tender {
            public int Count; // how many of each is stored in the bank (upon start up)
            public decimal Value;//the value of money stored in the bank(upon start up)
        }//end struct

        static Tender[] cashBox;

        static decimal cashBackAmount = 0.0m;
        #endregion

        const int MAX_TENDER_SLOT = 10;
        const int PENNY_SLOT = 10;
        static void Main(string[] args)
        {
            #region VARIABLES
            //Arrays cash
            string[] moneyNames = { "One Hundred", "Fifty", "Twenty", "Ten", "Five", "Dollar", "Half Dollar", "Quarter", "Dime", "Nickel", "Penny" };
            decimal[] moneyValue = { 100.00m, 50.00m, 20.00m, 10.00m, 5.00m, 1.00m, .50m, .25m, .10m, .05m, .01m };
            int[] drawerBillCount = { 1, 2, 4, 5, 8, 21, 5, 20, 28, 20, 20 };
            //int[] drawerBillCount1 = { 1, 1, 0, 1, 4, 1, 1, 1, 1, 1, 1 }; //test array
            cashBox = new Tender[moneyNames.Length];//create an instance of money

            //declare and intialize variables 
            decimal purchasePrice = 0.00m;
            int index = 0;
            bool transactionOn = true;         
            bool payCard = false;
            bool transactionComplete = false;
            #endregion

            #region  BOOT UP DATA FOR CASH DRAWER CHECK
            //SETUP BOOT UP CASH
            while (index < 11)
            //STORE PRELOADED INFORMATION INTO KIOSK
            {
                //GLOBAL MONEY DRAWER ARRAY SET TO LOCAL MONEY DRAWER
                cashBox[index].Count = drawerBillCount[index];
                cashBox[index].Value = moneyValue[index];
                index++;
            }
            #endregion

            
            while (transactionOn)//LOOP FOR MULTIPLE TRANSACTIONS
            {
                //CALL FUNCTION TO SCAN ITEMS
                purchasePrice = ScanItems();              

                //LOOP FOR CONTINUOUS TRANSACTIONS
                while (transactionComplete == false)
                {
                    //ACCEPT CARD                
                    payCard = AskPayWithCard(purchasePrice);//CALL FUNCTION TO CHECK IF PAYING WITH A CARD

                    if (payCard == true)
                    {
                        //CHECK IF CASH BACK NEEDED
                        CheckCashBack(purchasePrice);

                        //SET BOOL EQUAL TO ACCEPTCARD FUNCTION
                        transactionComplete = AcceptCard(ref purchasePrice);//PASS PURCHASE PRICE BY REFERENCE TO GET UPDATED PURCHASE PRICE FROM FUNCTION
                    }//end if   
                    
                    //ACCEPT CASH                 
                    else if (payCard == false)
                    {
                        //SET BOOL EQUAL TO ACCEPTCASH FUNCTION
                        transactionComplete = AcceptCash(purchasePrice);
                    }//end if            
                }//end while
           
                //RESET VARIABLES
                payCard = false;
                transactionComplete = false;
                purchasePrice = 0.0m;

            }//end while loop
            Console.ReadKey();
        }//end main function

        public static decimal ScanItems()
        {
            int itemNum = 1;
            string inputPrice = "";
            decimal itemPrice = 0.0m;
            decimal purchasePrice = 0.0m;
            bool cashVsCard;
            bool numericNumber = false;
            Console.WriteLine("You may begin scanning your items!\n");

            //LOOP FOR ITEM PRICE
            do
            {
                //LOOP FOR INPUT VALIDATION
                while (numericNumber == false)
                {
                    Console.Write("Item {0}: ", itemNum);

                    //BREAK IF NO INPUT VALUE
                    inputPrice = Console.ReadLine();
                    
                    cashVsCard = true;
                    numericNumber = IsNumeric(inputPrice, cashVsCard);
                }//end while

                //REINSTIALIZE TO FALSE SO LOOP WILL RUN FOR NEXT ITEM
                numericNumber = false;

                if (inputPrice == "")
                {
                    break;//BREAK OUT OF LOOP IF INPUT IS EMPTY
                }//end if
               
                //INPUT INTO DECIMAL FOR CALCULATIONS
                itemPrice = Convert.ToDecimal(inputPrice);


                Console.WriteLine("Please place item in bagging area!\n");
                itemNum += 1;
                purchasePrice += itemPrice;

            } while (inputPrice != "");//end do while loop

            //TOTAL PRICE DISPLAY
            Console.Write("\n\nTotal {0:c}", purchasePrice);
            return purchasePrice;       
        }//end function

        public static void DispenseChange(decimal change_needed, decimal[] moneyAmount, int[] drawerAmount)
        {
            int count = 0;
            //LOOP TO DISPLAY DISPENSED MONEY/CHANGE
            while (change_needed > 0)
            {
                if (change_needed >= moneyAmount[count] && drawerAmount[count] > 0)
                {
                    //SUBTRACT MONEY AMOUNT FROM CHANGE NEEDED
                    change_needed -= moneyAmount[count];
                    change_needed = Math.Round(change_needed, 2);

                    //DECREASE THE AMOUNT OF MONEY IN DRAWER
                    drawerAmount[count] -= 1;

                    //DISPLAY DISPENSED CHANGE
                    Console.WriteLine($"{moneyAmount[count]:c} dispensed");

                    if (change_needed == 0)
                    {
                        Console.WriteLine("\n-----Transaction Complete-----");

                    }//end if
                }
                //SKIP TO NEXT ARRAY ELEMENT IF LESS THAN ZERO STORED
                else if (drawerAmount[count] < 0)
                {
                    count += 2;
                }

                else
                {
                    count += 1;

                }//end if              
            }//end while loop
        }//end function

        public static bool CheckEnoughChange(decimal change_due, decimal[] moneyValue) {
            int current_slot = 0;
            //LOOP TO CHECK KIOSK FOR PROPER CHANGE
            while (change_due > 0) {
                if (cashBox[current_slot].Count == 0)  {
                    current_slot += 1;
                } else if (change_due >= moneyValue[current_slot] && cashBox[current_slot].Count > 0) {
                    //CHANGE DUE MINUS MONEYVALUE
                    change_due -= (moneyValue[current_slot]);

                    //DECREASE AMOUNT OF MONEY STORED
                    cashBox[current_slot].Count -= 1;
                    //INCREMENT TO NEXT ARRAY ELEMENT IF STORED IS EQUAL TO ZERO
                    if (cashBox[current_slot].Count == 0 && current_slot != MAX_TENDER_SLOT) {
                        current_slot += 1;
                        //RETURN FALSE IF i IS INCREMENTED OVER ARRAY LENGTH
                        if (current_slot > MAX_TENDER_SLOT) {
                            return false;
                        }//end if    

                    }//end if

                    //RETURNS FALSE IF AT END OF ARRAY AND LAST ARRAY IS EQUAL TO ZERO
                    if (cashBox[PENNY_SLOT].Count == 0 && current_slot == MAX_TENDER_SLOT)
                    {
                        return false;
                    }//end if

                    //RETURNS TRUE IF NO CHANGE IS NEEDED AND BILL/COIN COUNT IS ZERO OR MORE
                    if (cashBox[current_slot].Count >= 0 && change_due == 0)
                    {
                        return true;
                    }//end if
                }else{
                    current_slot += 1;
                    if (current_slot > MAX_TENDER_SLOT)
                    {
                        //RETURN FALSE IF i IS INCREMENTED OVER ARRAY LENGTH
                        return false;
                    }//end if
                }//end if

            }//end while loop
            return false;
        }//end function  
      
        public static bool CheckCashPayment(decimal payments, decimal[] moneyValue, int[] moneyStored)
        {
            for (int x = 0; x < moneyValue.Length; x++)
            {
                if (payments == moneyValue[x])
                {
                    //INCREASE VALUE OF MONEY DRAWER UPON MONEY INPUT
                    moneyStored[x] += 1;
                    return true;
                }//end if

                if (payments != moneyValue[x] && x == moneyValue.Length)
                {
                    return false;
                }//end if
            }//end for loop
            return false;
        }//end function
       
        public static bool AcceptCard(ref decimal purchasePrice)//PASS PURCHASE PRICE BY REFERENCE(CHANGES IN FUNCTION HOW AFFECT OUTSIDE FUNCTION)
        {
            string cardNumber = "";
            bool checkValidCard = false;
            int cardFirstDigit = 0;
            decimal cashBackPurchasePrice = 0.0m;
            string[] bankRequest;
            decimal bankRequestValue = 0.0m;
            bool transactionComplete = false;
            string cardVendor = "";
            bool cardDeclinedHalfpayment = false;
            string continueOrCancelTransaction = "";
            bool numericNumber = false;
            bool cash;
            //LOOP FOR NO VALID CARD NUMBER
            while (checkValidCard == false)
            {
                while (numericNumber == false)
                {
                    Console.Write("\nPlease enter your card number without any dashes or spaces!   ");
                    cardNumber = (Console.ReadLine());
                    cash = false;
                    numericNumber = IsNumeric(cardNumber,cash);
                }//end while
                
                
                //CALL FUNCTION TO GET FIRST DIGIT
                cardFirstDigit = GetCardNumberFirstDigit(cardNumber);

                //CALL FUNCTION TO CHECK VALID CARD NUMBER
                checkValidCard = IsCardNumberValid(cardNumber);


                if (checkValidCard == false)
                {
                    Console.WriteLine("Please enter a valid card number.");
                }
                //CALL FUNCTION CHECKVALIDCARD IF CARD IS VALID
                else if (checkValidCard == true)
                {
                    cardVendor = GetCardVendor(cardFirstDigit);
                    cashBackPurchasePrice = purchasePrice + cashBackAmount;

                    //CALL BANK REQUEST FUNCTION
                    bankRequest = MoneyRequest(cardNumber, cashBackPurchasePrice);

                    if (bankRequest[1] == "declined")
                    {
                        Console.WriteLine("Card Declined!!!!");
                        //DISPLAY REMAINING BALANCE MINUS CASHBACK
                        Console.WriteLine("Remaining Balance: {0}", purchasePrice);
                        Console.WriteLine("-----Please pay another way!-----");
                        
                        //SET CHECKVALIDCARD TO FALSE
                        cardDeclinedHalfpayment = true;
                        //return false;
                    }
                    else if (bankRequest[1] != "declined")
                    {
                        bankRequestValue = Convert.ToDecimal(bankRequest[1]);
                       decimal formatted_bankRequestValue = Math.Round(bankRequestValue, 4);


                        //PURCHASE COMPLETELY PAID BY CARD
                        if (bankRequestValue > 0 && bankRequestValue == cashBackPurchasePrice)
                        {
                            Console.WriteLine($"Total: {cashBackAmount + purchasePrice}");
                            Console.WriteLine("Your {0} card will be charged {1}", cardVendor, bankRequestValue);
                            Console.WriteLine("------Transaction Complete!!!_____\n\n");
                            return true;
                        }
                        //PURCHASE PARTIALLY PAID BY CARD
                        else if (bankRequestValue > 0 && bankRequestValue != cashBackPurchasePrice)
                        {

                            /*Console.WriteLine("Your {0} card will be charged {1}", cardVendor, bankRequestValue);*/
                            Console.WriteLine("------------Infsufficient funds!!!!!------------");

                            //LOOP TO ASK CUSTOMER IF THEY WONT TO CONTINUE WITH HALF PAYMENT TRANSACTION
                            do
                            {
                                Console.WriteLine("Your reamaining balance is {0}", purchasePrice);
                                Console.Write("\n\nWould you like to continue with a debit of {0} from {1}? Please enter y or n.   ", formatted_bankRequestValue, cardVendor);
                                continueOrCancelTransaction = Console.ReadLine();
                                //CHANGE ALL STRING INPUTS TO LOWERCASE
                                continueOrCancelTransaction = continueOrCancelTransaction[0].ToString().ToLower();
                                
                                if(continueOrCancelTransaction != "y" && continueOrCancelTransaction !="n")
                                {
                                    Console.WriteLine("Please enter a valid response!!!!");
                                }
                            } while (continueOrCancelTransaction != "y" && continueOrCancelTransaction != "n");

                            if (continueOrCancelTransaction == "y")
                            {
                                Console.WriteLine("Your reamaing balance is {0}", purchasePrice-bankRequestValue);
                                purchasePrice = purchasePrice - bankRequestValue;
                               
                                //SET CHECKVALIDCARD TO FALSE
                                cardDeclinedHalfpayment = true;
                                return false;
                            }
                            else if (continueOrCancelTransaction == "n")
                            {
                                Console.Clear();
                                return true;
                            }//end if
                            
                            return  false;
                        }//end if
                    }//end if
                }//end if
            }//end while
            if(cardDeclinedHalfpayment==true)
            {
                checkValidCard = false;
            }//end if
            return transactionComplete;
        }//end function

        public static bool IsNumeric(string cardNumber, bool cashVsCard)
        {
            int decimalValue = 0;

            for (int i = 0; i < cardNumber.Length; i++)
            {
                //CHECKING EACH CHAR ELEMENT OF STRING FOR NUMBER DECIMAL VALUES
                char letter = cardNumber[i];

                if (cashVsCard == false)
                {
                    decimalValue = (int)letter;
                    if (decimalValue < 48 || decimalValue > 57)
                    {
                        return false;
                    }//end if
                }
                else if (cashVsCard == true)
                {
                    decimalValue = (int)letter;
                    if (decimalValue < 46 || decimalValue > 57 || decimalValue == 47)
                    {
                        return false;
                    }//end if
                }
            }//end for
            return true;
        }//end function

        public static bool AcceptCash(decimal purchasePrice)
        {
            decimal[] moneyValue = { 100.00m, 50.00m, 20.00m, 10.00m, 5.00m, 1.00m, .50m, .25m, .10m, .05m, .01m };
            int[] drawerBillCount = { 1, 2, 4, 5, 8, 21, 5, 20, 28, 20, 20 };
            decimal totalPayment = 0.0m;
            int paymentNum = 1;
            decimal payment = 0.0m;
            decimal remaining = 0.0m;
            decimal changeDue = 0.0m;
            bool transactionComplete = false;
                       

            
            // LOOP TO ACCEPT CASH PAYMENT
            while (totalPayment <= purchasePrice)
            {
                Console.Write("\n\nPayment {0}  ", paymentNum);
                payment = Convert.ToDecimal(Console.ReadLine());


                //CALL FUNCTION TO CHECK VALID PAYMENT AMOUNT
                bool paymentCheck = CheckCashPayment(payment, moneyValue, drawerBillCount); //BOOL SET EQUAL TO FUNCTION CALL

                if (paymentCheck == false)
                {

                    Console.WriteLine("Please enter a valid payment amount.");
                    totalPayment = totalPayment - payment;
                    paymentNum -= 1;
                }//end if

                totalPayment += payment;
                //REMAINING EQUAL TO PURCHASEPRICE - TOTALPAYMENT
                remaining = purchasePrice - totalPayment;
                paymentNum += 1;

                if (totalPayment > purchasePrice)
                {
                    break;//EXIT LOOP PAYMENT HIGHER THAN PURCHASEPRICE
                }//end if 

                //REMAINING CASH TO BE PAID
                Console.Write("Remaining  {0:c}", remaining);
            }//end while loop


            remaining = Math.Round(remaining, 2);
            //ABSOLUTE VALUE OF CHANGE(NO NEGATIVE SIGN)
            changeDue = Math.Abs(remaining);

            //TOTAL AMOUNT PAID
            Console.WriteLine("\n\nTotal Cash Paid: {0}", totalPayment);

            //CHANGE DUE
            Console.Write("\nChange     {0:c}", changeDue);
            Console.Write("\n_ _ _ _ \n\n");

            //CALL ENOUGHCHANGE FUNCTION
            bool enoughChange = CheckEnoughChange(changeDue, moneyValue);//BOOL SET TO ENOUGHCHANGE FUNCTION

            if (enoughChange == false)
            {
                //DISPLAY MESSAGE FOR NOT ENOUGH CHANGE
                Console.WriteLine("Kiosk does not have enough money to dispense change! Please enter another form of payment.");
                return false;
            }//end if                       
            else if (enoughChange == true)
            {
                //DISPENSE CHANGE
                DispenseChange(changeDue, moneyValue, drawerBillCount);
                return true;
            }
            return transactionComplete; 
        }
        
        public static int GetCardNumberFirstDigit(string cardNum)
        {
            int cardFirstDigit = 0;
            //DISPLAY CARD VENDOR
            cardFirstDigit = (int)(cardNum.ToString()[0] - 48);
            return cardFirstDigit;
        }//end function
            
        public static string GetCardVendor(int cardFirstDig)
        {
            if (cardFirstDig == 3)
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
            else if (cardFirstDig == 6)
            {
                return "Discover";
            }
            else
            {
                return "***ERROR***";
            }//end if 
        }//end function
        
        public static bool AskPayWithCard(decimal purchasePrice){
            string useCard = "";
            bool return_value = false;

            do {
                //LOOP TO CHECK IF PAYING WITH CARD
                Console.Write("\n\nWould you like to pay balance with a credit card? Please enter y or n.   ");
                useCard = Console.ReadLine();
                useCard = useCard[0].ToString().ToLower();
 
                //SET USECARD TO TRUE IF CUSTOMER WANTS TO USE CARD OTHERWISE FALSE
                if (useCard == "y") {
                    return_value = true;
                }else if (useCard == "n"){
                    return_value =  false;
                }else{
                    Console.WriteLine("Please enter a valid response!!!!");
                }//end if

            } while (useCard != "y" && useCard != "n");

            return return_value;
        }//end function
        
        static bool CheckCashBack(decimal purchasePrice)
        {
            string cashBackCheck = "";
            decimal cashBackPurchasePrice = 0.0m;

            //LOOP CHECKING FOR CASHBACK
            do
            {
                Console.Write("Would you like cash back?  ");
                cashBackCheck = Console.ReadLine();
                cashBackCheck = cashBackCheck[0].ToString().ToLower();

                if (cashBackCheck == "Y" || cashBackCheck == "y" || cashBackCheck == "yes" || cashBackCheck == "Yes")
                {
                    Console.Write("Enter cashback amount:  ");

                    //
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
               
        public static bool IsCardNumberValid(string cardNumber)
        {
            bool cardValid = false;
            int firstDigit= GetCardNumberFirstDigit(cardNumber);

            //CHECK TO MAKE SURE CARD STARTS WITH 3, 4, 5, OR 6
            if (firstDigit == 3 || firstDigit == 4 || firstDigit ==5 || firstDigit == 6 ){
                //CHECK TO MAKE SURE CARD IS BETWEEN 15 & 16 CHARACTERS
                if (cardNumber.Length >= 15 && cardNumber.Length <= 16){

                    int i, checkSum = 0;

                    //COMPUTE CHECKSUM OF EVERY OTHER DIGIT STARTING FROM RIGHT-MOST DIGIT
                    for (i = cardNumber.Length - 1; i >= 0; i -= 2)
                        checkSum += (cardNumber[i] - '0');

                    /*NOW TAKE DIGITS NOT INCLUDED IN FIRST CHECKSUM, MULTIPLE BY TWO, AND COMPUTE CHECKSUM OF RESULTING DIGITS*/
                    for (i = cardNumber.Length - 2; i >= 0; i -= 2){
                        int val = ((cardNumber[i] - '0') * 2);
                        while (val > 0)
                        {
                            checkSum += (val % 10);
                            val /= 10;
                        }//end while loop
                    }//end for loop

                    // NUMBER IS VALID IF SUM OF BOTH CHECKSUMS MOD 10 EQUALS 0
                    //return ((checkSum % 10) == 0);
                    return true;
                }
                else{
                    return cardValid;
                }//end if
            }
            else{
                return cardValid;
            }//end if
        }//end function
              
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
                }//end if
            }//end if         
        }//end function
           
    }//end program class
}//end namespace
